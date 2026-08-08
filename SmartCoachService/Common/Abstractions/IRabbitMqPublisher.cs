namespace SmartCoachService.Common.Abstractions;

public interface IRabbitMqPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default) where TEvent : class;
}
