namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Request;

public class LogWeightRequest
{
    public decimal Weight { get; set; }
    
    public DateTimeOffset Date { get; set; }
    
    public string? Notes { get; set; }
}