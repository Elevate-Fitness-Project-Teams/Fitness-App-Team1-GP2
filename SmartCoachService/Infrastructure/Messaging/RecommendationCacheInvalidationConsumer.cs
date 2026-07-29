using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartCoachService.Common.Abstractions;
using SmartCoachService.Infrastructure.Messaging.Events;
using System.Text;
using System.Text.Json;

namespace SmartCoachService.Infrastructure.Messaging;

/// <summary>
/// Background consumer listening for events raised by Profile/FCE/Workout/Nutrition/Progress
/// (e.g. "profile.updated", "workout.plan.changed") and eagerly invalidates the caller's
/// RecommendationCache row so the next GET /home rebuilds a fresh payload instead of serving stale data.
/// </summary>
public sealed class RecommendationCacheInvalidationConsumer : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecommendationCacheInvalidationConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    private static readonly string[] RoutingKeysOfInterest =
    {
        "profile.updated",
        "fce.metrics.calculated",
        "workout.plan.changed",
        "nutrition.recommendations.generated",
        "progress.updated"
    };

    public RecommendationCacheInvalidationConsumer(
        IOptions<RabbitMqSettings> settings,
        IServiceScopeFactory scopeFactory,
        ILogger<RecommendationCacheInvalidationConsumer> logger)
    {
        _settings = settings.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection("smart-coach-service-cache-invalidation-consumer");
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Topic, durable: true);

            var queueName = _channel.QueueDeclare("smart-coach.home-feed-cache-invalidation", durable: true, exclusive: false, autoDelete: false).QueueName;
            foreach (var key in RoutingKeysOfInterest)
                _channel.QueueBind(queueName, _settings.ExchangeName, key);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);

                    if (payload is not null && payload.TryGetValue("UserId", out var userIdElement) &&
                        Guid.TryParse(userIdElement.ToString(), out var userId))
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var cacheEntry = await unitOfWork.RecommendationCaches.FirstOrDefaultAsync(c => c.UserId == userId, stoppingToken);
                        if (cacheEntry is not null)
                        {
                            cacheEntry.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
                            unitOfWork.RecommendationCaches.Update(cacheEntry);
                            await unitOfWork.SaveChangesAsync(stoppingToken);
                        }
                    }

                    _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed processing cache invalidation message");
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queueName, autoAck: false, consumer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start RabbitMQ consumer; Home Feed will rely on TTL-only expiry.");
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
