using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;

namespace FCEService.Features.CalculatedMetrics.Orchestrators
{
    public record CalculatedMetricsOrchestrator(int userId) : IRequest<RequestResult<CalculatedMetricsResponseDTO>>;
    public class CalculatedMetricsOrchestratorHandler : IRequestHandler<CalculatedMetricsOrchestrator, RequestResult<CalculatedMetricsResponseDTO>>
    {
        private readonly IMediator _mediator;
        public CalculatedMetricsOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResult<CalculatedMetricsResponseDTO>> Handle(CalculatedMetricsOrchestrator request, CancellationToken cancellationToken)
        {
            var queryResult = await _mediator.Send(new Queries.GetDetailsandCalculatedMetricsQuery(request.userId), cancellationToken);
            if (!queryResult.IsSuccess)
            {
                return RequestResult<CalculatedMetricsResponseDTO>.Failure(queryResult.Message);
            }
            var calculatedMetrics = queryResult.Data;
            await _mediator.Send(new Commands.CalculatedMetricsCommand(calculatedMetrics), cancellationToken);
            var responseDTO = new CalculatedMetricsResponseDTO(
                request.userId,
                calculatedMetrics.BMR,
                calculatedMetrics.TDEE,
                calculatedMetrics.CalorieTarget,
                calculatedMetrics.BMRRange,
                calculatedMetrics.BMRStatus
            );
            return RequestResult<CalculatedMetricsResponseDTO>.Success(responseDTO, "Calculated metrics retrieved successfully.");
        }
    }
    public record CalculatedMetricsResponseDTO(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
}
