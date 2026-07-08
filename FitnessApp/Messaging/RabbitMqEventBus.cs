using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using WorkoutService.Contracts;
using WorkoutService.Contracts.Events;

namespace WorkoutService.Messaging
{
    public sealed class RabbitMqEventBus(IOptions<RabbitMqOptions> options) : IEventBus
    {
        private readonly RabbitMqOptions _options = options.Value;
        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);

            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                  exchange: _options.ExchangeName,
                  type: ExchangeType.Topic,
                  durable: true,
                  autoDelete: false,
                  arguments: null,
                  cancellationToken: cancellationToken);

            var routingKey = GetRoutingKey<TEvent>();
            var messageBody = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@event);

            await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            body: messageBody,
            cancellationToken: cancellationToken);
        }
        private static string GetRoutingKey<TEvent>()
        {
            if (typeof(TEvent) == typeof(WorkoutSessionStartedEvent))
            {
                return "workout.session.started";
            }

            return typeof(TEvent).Name;
        }
    }
}
