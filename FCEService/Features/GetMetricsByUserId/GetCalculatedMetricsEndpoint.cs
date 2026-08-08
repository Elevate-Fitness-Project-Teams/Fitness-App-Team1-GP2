using Azure.Core;
using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FCEService.Features.CalculatedMetrics.Orchestrators;
using FCEService.Features.GetMetricsByUserId.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.GetMetricsByUserId
{
    [Route("api/v1/fitness/metrics")]
    [ApiController]
    public class GetCalculatedMetricsEndpoint(IMediator _mediator) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<EndPointResponse<GetCalculatedMetricsResponseViewModel>> GetCalculatedMetrics(int userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCalculatedMetricsByUseridQuery(userId), cancellationToken);
            if (!result.IsSuccess)
            {
                return EndPointResponse<GetCalculatedMetricsResponseViewModel>.Failure(result.Message, ResponseErrorCode.InternalServerError);
            }
            var _calculatedMetrics = result.Data;
            var _response = new GetCalculatedMetricsResponseViewModel(
                userId,
                _calculatedMetrics.BMR,
                _calculatedMetrics.TDEE,
                _calculatedMetrics.CalorieTarget,
                _calculatedMetrics.BMRRange.ToString(),
                _calculatedMetrics.BMRStatus.ToString()
            );
            return EndPointResponse<GetCalculatedMetricsResponseViewModel>.Success(_response, "Calculated metrics is already exist and retrieved successfully.");
        }
    }

    public record GetCalculatedMetricsResponseViewModel(int UserId, double BMR, double TDEE, double CalorieTarget, string BMRRange, string BMRStatus);
}
