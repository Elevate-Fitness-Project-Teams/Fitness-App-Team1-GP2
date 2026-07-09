using FCEService.Common.Abstract;
using FCEService.Common.ViewModels;
using FCEService.Domain.Aggregates;
using FCEService.Domain.Enums;
using FCEService.Domain.ValueObject;
using FluentValidation;
using MediatR;

namespace FCEService.Features.WeightGoalActivity.Commands
{
    public record WeightGoalActivityCommand(int userId, PhysicalStats physicalStats, Activity activity, int WorkoutDays,bool IsActive) : IRequest<RequestResult<int>>;

    public class WeightGoalActivityCommandHandler : IRequestHandler<WeightGoalActivityCommand, RequestResult<int>>
    {
        IGenericRepository<UserFitnessStats> _repository;
        public WeightGoalActivityCommandHandler(IGenericRepository<UserFitnessStats> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<int>> Handle(WeightGoalActivityCommand request, CancellationToken cancellationToken)
        {
            _repository.Add(new UserFitnessStats
            {
                userId = request.userId,
                physicalStats = request.physicalStats,
                activity = request.activity,
                WorkoutDays = request.WorkoutDays,
                IsActive = request.IsActive
            });     
            await _repository.SaveChangeAsync(cancellationToken);
            return RequestResult<int>.Success(request.userId);
        }
    }

    public class WeightGoalActivityCommandValidator : AbstractValidator<WeightGoalActivityCommand>
    {
        public WeightGoalActivityCommandValidator()
        {
            RuleFor(x => x.userId).GreaterThan(0).WithMessage("UserId must be greater than 0");
            RuleFor(x => x.physicalStats).NotNull().WithMessage("PhysicalStats cannot be null");
            RuleFor(x => x.physicalStats.Weight).InclusiveBetween(40, 200).WithMessage("Weight must be between 40 and 200");
            RuleFor(x => x.physicalStats.Height).InclusiveBetween(140, 220).WithMessage("Height must be between 140 and 220");
            RuleFor(x => x.physicalStats.Gender).IsInEnum().WithMessage("Gender must be a valid enum value");
            RuleFor(x => x.physicalStats.Age).InclusiveBetween(16, 100).WithMessage("Age must be between 16 and 100");
            RuleFor(x => x.activity).IsInEnum().WithMessage("Activity must be a valid enum value");
            RuleFor(x => x.WorkoutDays).InclusiveBetween(0, 7).WithMessage("WorkoutDays must be between 0 and 7");
        }
    }
}
