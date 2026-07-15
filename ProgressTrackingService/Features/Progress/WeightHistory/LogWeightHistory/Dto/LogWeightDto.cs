namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Dto;

public class LogWeightDto
{
    public decimal Bmi { get; set; }
    
    public decimal DifferenceFromPrevious { get; set; }
    
    public decimal TotalWeightLost { get; set; }
}