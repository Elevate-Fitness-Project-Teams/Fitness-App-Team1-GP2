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
            var result = await _mediator.Send(new GetMetricsByUserId.Queries.GetCalculatedMetricsByUseridQuery(request.userId), cancellationToken);
            if(result.IsSuccess)
            {
                var _calculatedMetrics=result.Data;
                var _responseDTO = new CalculatedMetricsResponseDTO(
                    request.userId,
                    _calculatedMetrics.BMR,
                    _calculatedMetrics.TDEE,
                    _calculatedMetrics.CalorieTarget,
                    _calculatedMetrics.BMRRange,
                    _calculatedMetrics.BMRStatus
                );
                return RequestResult<CalculatedMetricsResponseDTO>.Success(_responseDTO, "Calculated metrics is already exist and retrieved successfully.");
            }
            var queryResult = await _mediator.Send(new Queries.GetDetailsandCalculatedMetricsQuery(request.userId), cancellationToken);
            var calculatedMetrics = queryResult.Data;
            if (!queryResult.IsSuccess)
            {
                return RequestResult<CalculatedMetricsResponseDTO>.Failure(queryResult.Message);
            }
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
