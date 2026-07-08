using FluentValidation;

namespace WorkoutService.Feature.WorkoutPlans.GetWorkoutPlanById;

public sealed class GetWorkoutPlanByIdValidator : AbstractValidator<GetWorkoutPlanByIdQuery>
{
    public GetWorkoutPlanByIdValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan id is required.");
    }
}