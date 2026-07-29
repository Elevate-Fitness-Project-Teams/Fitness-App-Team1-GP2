using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartCoachService.Domain.Entities;
using SmartCoachService.Domain.Events;
using SmartCoachService.Infrastructure.Persistence;
using System.Text;
using System.Text.Json;

namespace SmartCoachService.Infrastructure.Messaging;

// ── Base class to reduce boilerplate ─────────────────────────────────────────────
public abstract class RabbitConsumerBase<TEvent> : BackgroundService
{
    private readonly string _exchange;
    private readonly string _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private readonly IModel _channel;

    protected RabbitConsumerBase(
        string exchange, string queue,
        IConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger logger)
    {
        _exchange = exchange;
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;

        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_queue, _exchange, routingKey: string.Empty);
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
                var evt = JsonSerializer.Deserialize<TEvent>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (evt is not null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SmartCoachDbContext>();
                    await HandleAsync(evt, db, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);
                }

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event on {Queue}, requeueing.", _queue);
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_queue, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    protected abstract Task HandleAsync(TEvent evt, SmartCoachDbContext db, CancellationToken ct);

    public override void Dispose() { _channel?.Close(); base.Dispose(); }
}

// ── Consumer 1: CalorieTargetCalculatedEvent (from FCE Service) ───────────────────
public sealed class FceContextConsumer : RabbitConsumerBase<CalorieTargetCalculatedEvent>
{
    public FceContextConsumer(IConnection conn, IServiceScopeFactory sf, ILogger<FceContextConsumer> log)
        : base("fce.calorie-target.calculated", "smart-coach.fce-context", conn, sf, log) { }

    protected override async Task HandleAsync(
        CalorieTargetCalculatedEvent evt, SmartCoachDbContext db, CancellationToken ct)
    {
        var existing = await db.UserFceContexts.FindAsync(new object[] { evt.UserId }, ct);
        if (existing is null)
        {
            db.UserFceContexts.Add(new UserFceContext
            {
                UserId = evt.UserId,
                DailyGoalCalories = evt.DailyGoalCalories,
                MinProteinGrams = evt.MinProteinGrams,
                FitnessGoal = evt.FitnessGoal,
                UpdatedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.DailyGoalCalories = evt.DailyGoalCalories;
            existing.MinProteinGrams = evt.MinProteinGrams;
            existing.FitnessGoal = evt.FitnessGoal;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}

// ── Consumer 2: ProgressUpdatedEvent (from Progress Service) ─────────────────────
public sealed class ProgressContextConsumer : RabbitConsumerBase<ProgressUpdatedEvent>
{
    public ProgressContextConsumer(IConnection conn, IServiceScopeFactory sf, ILogger<ProgressContextConsumer> log)
        : base("progress.snapshot.updated", "smart-coach.progress-context", conn, sf, log) { }

    protected override async Task HandleAsync(
        ProgressUpdatedEvent evt, SmartCoachDbContext db, CancellationToken ct)
    {
        var existing = await db.UserProgressContexts.FindAsync(new object[] { evt.UserId }, ct);
        if (existing is null)
        {
            db.UserProgressContexts.Add(new UserProgressContext
            {
                UserId = evt.UserId,
                CurrentWeightKg = evt.CurrentWeightKg,
                BodyFatPercentage = evt.BodyFatPercentage,
                CompletedWorkoutsLast30Days = evt.CompletedWorkoutsLast30Days,
                UpdatedAtUtc = evt.UpdatedAtUtc
            });
        }
        else
        {
            existing.CurrentWeightKg = evt.CurrentWeightKg;
            existing.BodyFatPercentage = evt.BodyFatPercentage;
            existing.CompletedWorkoutsLast30Days = evt.CompletedWorkoutsLast30Days;
            existing.UpdatedAtUtc = evt.UpdatedAtUtc;
        }
    }
}
