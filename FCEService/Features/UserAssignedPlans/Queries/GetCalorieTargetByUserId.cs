using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.UserAssignedPlans.Queries
{
    public record GetCalorieTargetByUserId(int userId) : IRequest<RequestResult<double>>;
    public class GetCalorieTargetByUserIdHandler : IRequestHandler<GetCalorieTargetByUserId, RequestResult<double>>
    {
        private readonly IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> _repository;
        public GetCalorieTargetByUserIdHandler(IGenericRepository<FCEService.Domain.Aggregates.CalculatedMetrics> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<double>> Handle(GetCalorieTargetByUserId request, CancellationToken cancellationToken)
        {
            var calorieTarget = await _repository.Get(x => x.UserId == request.userId)
                .Select(s => s.CalorieTarget)
                .FirstOrDefaultAsync(cancellationToken);
            return RequestResult<double>.Success(calorieTarget, "Calorie target retrieved successfully.");
        }
    }
}
