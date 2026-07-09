using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FCEService.Features.CalculatedMetrics.Orchestrators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.CalculatedMetrics
{
    [Route("api/v1/fitness/calculate")]
    [ApiController]
    public class CalculatedMetricsEndpoint(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("{userId}")]
        public async Task<EndPointResponse<CalculatedMetricsResponseViewModel>> CalculatedMetrics(int userId)
        {
            var request = await _mediator.Send(new CalculatedMetricsOrchestrator(userId));
            var retrivedData = request.Data;
            var response = new CalculatedMetricsResponseViewModel(
                UserId:retrivedData.UserId,
                BMR: retrivedData.BMR,
                TDEE: retrivedData.TDEE,
                CalorieTarget: retrivedData.CalorieTarget,
                BMRRange: retrivedData.BMRRange,
                BMRStatus: retrivedData.BMRStatus
            );
            return (request == null)
                ? EndPointResponse<CalculatedMetricsResponseViewModel>.Failure("Failed to calculate metrics", ResponseErrorCode.CalculatedMetricsError)
                : EndPointResponse<CalculatedMetricsResponseViewModel>.Success(response, "Calculated metrics retrieved successfully.");

        }

        public record CalculatedMetricsResponseViewModel(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
    }
}
