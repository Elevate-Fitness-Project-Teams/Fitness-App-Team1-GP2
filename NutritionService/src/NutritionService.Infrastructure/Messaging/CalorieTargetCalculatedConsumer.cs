using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Events;
using NutritionService.Infrastructure.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NutritionService.Infrastructure.Messaging;

public class CalorieTargetCalculatedConsumer : BackgroundService
{
    private const string ExchangeName = "fce.calorie-target.calculated";
    private const string QueueName = "nutrition-service.calorie-target.calculated";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CalorieTargetCalculatedConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public CalorieTargetCalculatedConsumer(
        IServiceScopeFactory scopeFactory,
        ILogger<CalorieTargetCalculatedConsumer> logger,
        IConnection rabbitConnection)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _connection = rabbitConnection;

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(QueueName, ExchangeName, routingKey: string.Empty);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<CalorieTargetCalculatedEvent>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (evt is not null)
                {
                    await UpsertAsync(evt, stoppingToken);
                }

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process CalorieTargetCalculatedEvent, requeueing.");
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(QueueName, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    private async Task UpsertAsync(CalorieTargetCalculatedEvent evt, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NutritionDbContext>();

        var existing = await db.UserCalorieTargets.FindAsync(new object[] { evt.UserId }, ct);
        if (existing is null)
        {
            db.UserCalorieTargets.Add(new UserCalorieTarget
            {
                UserId = evt.UserId,
                DailyGoalCalories = evt.DailyGoalCalories,
                MinProteinGrams = evt.MinProteinGrams,
                CalculatedAtUtc = evt.CalculatedAtUtc,
                UpdatedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.DailyGoalCalories = evt.DailyGoalCalories;
            existing.MinProteinGrams = evt.MinProteinGrams;
            existing.CalculatedAtUtc = evt.CalculatedAtUtc;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        _logger.LogInformation("Projected CalorieTarget for user {UserId} into local read model.", evt.UserId);
    }

    public override void Dispose()
    {
        _channel?.Close();
        base.Dispose();
    }
}
