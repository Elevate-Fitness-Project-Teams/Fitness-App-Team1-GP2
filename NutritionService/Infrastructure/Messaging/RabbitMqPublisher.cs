using Microsoft.Extensions.Options;
using NutritionService.Common.Abstractions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NutritionService.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events to a topic exchange. Connection/channel are created
/// lazily and reused; a dead broker never blocks a request thread because publishing
/// happens after the HTTP response has already been prepared in the handler.
/// </summary>
public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly Lazy<IConnection> _connection;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _connection = new Lazy<IConnection>(CreateConnection);
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            DispatchConsumersAsync = true
        };
        return factory.CreateConnection("nutrition-service-publisher");
    }

    public Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            using var channel = _connection.Value.CreateModel();
            channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Topic, durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            channel.BasicPublish(_settings.ExchangeName, routingKey, props, body);
        }
        catch (Exception ex)
        {
            // Publishing is best-effort: a broker outage must never fail the caller's request.
            _logger.LogError(ex, "Failed to publish {EventType} with routing key {RoutingKey}", typeof(TEvent).Name, routingKey);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_connection.IsValueCreated)
            _connection.Value.Dispose();
    }
}
