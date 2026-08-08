namespace ProgressTrackingService.Domain.Entities;

public class BaseEntity<TType>
{
    public TType Id { get; set; } = default!;
}