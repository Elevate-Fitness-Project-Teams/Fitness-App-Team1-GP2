namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.ViewModel;

public class LogWeightViewModel
{
    public decimal Bmi { get; set; }
    
    public decimal DifferenceFromPrevious { get; set; }
    
    public decimal TotalWeightLost { get; set; }
}