namespace NutritionService.Common.Abstractions;

/// <summary>Thin abstraction over RabbitMQ so features never depend on the broker client directly.</summary>
public interface IRabbitMqPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : class;
}
