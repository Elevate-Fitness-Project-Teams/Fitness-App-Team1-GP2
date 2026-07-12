namespace ProgressTrackingService.Domain.Entities;

public class WeightHistory : BaseEntity<int>
{
    public decimal Weight { get; set; }
    
    public DateTimeOffset Date { get; set; }
    
    public string? Notes { get; set; }
    
    #region Relations

    public int UserId { get; set; }

    #endregion
}