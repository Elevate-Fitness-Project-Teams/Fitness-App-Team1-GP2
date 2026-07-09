namespace NotificationService.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "progress.events";
    public string QueueName { get; set; } = "notification.achievement-earned";
    public string RoutingKey { get; set; } = "achievement_earned";
}