namespace WorkoutService.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "host.docker.internal";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "workout.events";
}