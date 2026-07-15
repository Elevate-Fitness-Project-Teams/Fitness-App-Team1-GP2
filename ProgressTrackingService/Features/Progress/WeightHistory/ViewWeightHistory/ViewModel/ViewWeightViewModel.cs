using ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.Dto;

namespace ProgressTrackingService.Features.Progress.WeightHistory.ViewWeightHistory.ViewModel;

public class ViewWeightViewModel
{
    public List<ViewWeightHistoryDto> WeightHistory { get; set; } = new List<ViewWeightHistoryDto>();
}