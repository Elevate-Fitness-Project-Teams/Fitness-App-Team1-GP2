namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Dto;

public class ViewWeightHistoryDto
{
    public decimal Weight { get; set; }
    
    public DateTimeOffset Date { get; set; }
    
    public string? Notes { get; set; }

    public int UserId { get; set; }
}