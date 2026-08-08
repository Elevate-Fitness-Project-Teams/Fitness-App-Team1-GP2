using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FCEService.Features.RecalulateMatrics.Orchestrators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.RecalulateMatrics
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecalculateMatricsEndpoint : ControllerBase
    {
        [HttpPost]
        public async Task<EndPointResponse<ReCalculatedMetricsResponseViewModel>> RecalculateMatrics(RecalculateMatricsRequestViewModel request, [FromServices] MediatR.IMediator _mediator)
        {
            var result = await _mediator.Send(new RecalculateMatricsOrchestrator(request.UserId, request.NewWeight, request.Reason));
            if (!result.IsSuccess)
            {
                return EndPointResponse<ReCalculatedMetricsResponseViewModel>.Failure(result.Message);
            }
            var retrivedData= result.Data;
            var response = new ReCalculatedMetricsResponseViewModel(
                retrivedData.UserId,
                retrivedData.BMR,
                retrivedData.TDEE,
                retrivedData.CalorieTarget,
                retrivedData.BMRRange,
                retrivedData.BMRStatus
                );
                
            return EndPointResponse<ReCalculatedMetricsResponseViewModel>.Success(response);
        }

    }
    public record ReCalculatedMetricsResponseViewModel(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
    public record RecalculateMatricsRequestViewModel(int UserId, double NewWeight, string Reason);
}
