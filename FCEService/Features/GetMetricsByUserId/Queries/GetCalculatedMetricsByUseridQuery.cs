using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCEService.Features.GetMetricsByUserId.Queries
{
    public record GetCalculatedMetricsByUseridQuery(int userId) : IRequest<RequestResult<calculatedMatricsDTO>>;
    public class GetCalculatedMetricsByUseridQueryQueryHandler : IRequestHandler<GetCalculatedMetricsByUseridQuery, RequestResult<calculatedMatricsDTO>>
    {
        private readonly IGenericRepository<Domain.Aggregates.CalculatedMetrics> _repository;
        public GetCalculatedMetricsByUseridQueryQueryHandler(IGenericRepository<Domain.Aggregates.CalculatedMetrics> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<calculatedMatricsDTO>> Handle(GetCalculatedMetricsByUseridQuery request, CancellationToken cancellationToken)
        {
            var calculatedMetricsDto = await _repository.Get(x => x.UserId == request.userId)
                .Select(x => new calculatedMatricsDTO(
                    x.UserId,
                    x.BMR,
                    x.TDEE,
                    x.CalorieTarget,
                    x.BMRRange,
                    x.BMRStatus
                        )).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (calculatedMetricsDto == null)
            {
                return RequestResult<calculatedMatricsDTO>.Failure("Calculated metrics not found for the specified user.");
            }
            return RequestResult<calculatedMatricsDTO>.Success(calculatedMetricsDto, "Calculated metrics retrieved successfully.");
        }
    }
    public record calculatedMatricsDTO(int UserId, double BMR, double TDEE, double CalorieTarget, BMRRange BMRRange, BMRStatus BMRStatus);
}
