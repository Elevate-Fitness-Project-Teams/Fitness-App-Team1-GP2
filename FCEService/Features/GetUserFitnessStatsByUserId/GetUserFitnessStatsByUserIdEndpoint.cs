using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCEService.Features.GetUserFitnessStatsByUserId
{
    [Route("api/v1/fitness/stats")]
    [ApiController]
    public class GetUserFitnessStatsByUserIdEndpoint : ControllerBase
    {
        private readonly IMediator _mediator;
        public GetUserFitnessStatsByUserIdEndpoint(IMediator mediator) 
        { 
            _mediator = mediator;
        }
        [HttpGet("{userId}")]
        public async Task<EndPointResponse<GetUserFitnessStatsResponseViewModel>> GetUserFitnessStats(int userId)
        {
            var result = await _mediator.Send(new Queries.GetUserFitnessStatsByUserId(userId));
            if (!result.IsSuccess) 
            {
                return EndPointResponse<GetUserFitnessStatsResponseViewModel>.Failure(result.Message);
            }
            var userFitnessStats = result.Data;
            var responseViewModel = new GetUserFitnessStatsResponseViewModel(
                userFitnessStats.userId,
                userFitnessStats.physicalStats,
                userFitnessStats.goal,
                userFitnessStats.activity,
                userFitnessStats.WorkoutDays,
                userFitnessStats.IsActive
            );
            return EndPointResponse<GetUserFitnessStatsResponseViewModel>.Success(responseViewModel);
        }
    }

    public record GetUserFitnessStatsResponseViewModel(int userId, PhysicalStats physicalStats, Goal goal, Activity activity, int WorkoutDays, bool IsActive);

}
