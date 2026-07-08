using FluentValidation;

namespace WorkoutService.Feature.Workouts.GetWorkoutsByPlan
{
    public class GetWorkoutsByPlanValidator : AbstractValidator<GetWorkoutsByPlanQuery>
    {
        public GetWorkoutsByPlanValidator()
        {
            RuleFor(x => x.planId)
                .NotEmpty().WithMessage("Plan ID must not be empty.")
                .MaximumLength(50).WithMessage("Plan ID must not exceed 50 characters.");
        }
}
}
