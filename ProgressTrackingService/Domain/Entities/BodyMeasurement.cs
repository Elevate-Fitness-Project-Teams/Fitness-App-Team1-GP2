namespace ProgressTrackingService.Domain.Entities;

public class BodyMeasurement : BaseEntity<int>
{
    public decimal? Neck { get; set; }
    
    public decimal? Chest { get; set; }
    
    public decimal? Biceps { get; set; }
    
    public decimal? Waist { get; set; }
    
    public decimal? Hips { get; set; }
    
    public decimal? Thighs { get; set; }
    
    public DateTimeOffset RecordedAt { get; set; }

    #region Relations

    public int UserId { get; set; }

    #endregion
}