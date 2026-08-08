namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Events;

public class LogWeightEvent
{
    public int UserId { get; set; }
    
    public decimal Bmi { get; set; }
    
    public decimal DifferenceFromPrevious { get; set; }
    
    public decimal TotalWeightLost { get; set; }
}