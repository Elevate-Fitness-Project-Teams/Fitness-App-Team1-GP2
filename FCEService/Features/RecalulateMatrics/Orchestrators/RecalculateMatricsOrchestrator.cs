using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using static FCEService.Features.RecalulateMatrics.Orchestrators.RecalculateMatricsOrchestratorHandler;

namespace FCEService.Features.RecalulateMatrics.Orchestrators
{
    public record RecalculateMatricsOrchestrator(int userId, double newWeight,string reason) : IRequest<RequestResult<ReCalculatedMetricsResponseDTO>>;
    public class RecalculateMatricsOrchestratorHandler : IRequestHandler<RecalculateMatricsOrchestrator, RequestResult<ReCalculatedMetricsResponseDTO>>
    {
        private readonly IMediator _mediator;
        public RecalculateMatricsOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResult<ReCalculatedMetricsResponseDTO>> Handle(RecalculateMatricsOrchestrator request, CancellationToken cancellationToken)
        {
            var updateWeightResult = await _mediator.Send(new Commands.UpdateUserFitnessStatsCommand(request.userId, request.newWeight), cancellationToken);
            if (!updateWeightResult.IsSuccess)
            {
                return RequestResult<ReCalculatedMetricsResponseDTO>.Failure(updateWeightResult.Message);
            }
            var queryResult = await _mediator.Send(new Features.CalculatedMetrics.Queries.GetDetailsandCalculatedMetricsQuery(request.userId), cancellationToken);
            if (!queryResult.IsSuccess)
            {
                return RequestResult<ReCalculatedMetricsResponseDTO>.Failure(queryResult.Message);
            }
            var calculatedMetrics = queryResult.Data;
            var recalculateMetricsResult = await _mediator.Send(new Commands.RecalculateMatricsCommand(calculatedMetrics), cancellationToken);
            if (!recalculateMetricsResult.IsSuccess)
            {
                return RequestResult<ReCalculatedMetricsResponseDTO>.Failure(recalculateMetricsResult.Message);
            }
            // i will use massage to assign the reason for recalculation it should assign a new plan and add details to plan historeis

            return RequestResult<ReCalculatedMetricsResponseDTO>
                .Success(new ReCalculatedMetricsResponseDTO(
                    request.userId, 
                    calculatedMetrics.BMR, 
                    calculatedMetrics.TDEE, 
                    calculatedMetrics.CalorieTarget, 
                    calculatedMetrics.BMRRange, 
                    calculatedMetrics.BMRStatus), 
                    "User fitness stats and metrics recalculated successfully.");
        }
        public record ReCalculatedMetricsResponseDTO(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
    }
}
