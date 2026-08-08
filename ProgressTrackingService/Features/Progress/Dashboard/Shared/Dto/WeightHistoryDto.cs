namespace ProgressTrackingService.Features.Progress.Dashboard.Shared.Dto;

public class WeightHistoryDto
{
    public decimal Weight { get; set; }
    
    public DateTimeOffset Date { get; set; }
    
    public string? Notes { get; set; }
    
    public int UserId { get; set; }
}