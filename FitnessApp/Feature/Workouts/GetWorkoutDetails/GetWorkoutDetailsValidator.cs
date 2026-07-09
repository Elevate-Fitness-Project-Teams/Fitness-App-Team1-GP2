using FluentValidation;

namespace WorkoutService.Feature.Workouts.GetWorkoutDetails
{
    public class GetWorkoutDetailsValidator : AbstractValidator<GetWorkoutDetailsQuery>
    {
        public GetWorkoutDetailsValidator()
        {
            RuleFor(x => x.workoutId)
                .GreaterThan(0).WithMessage("Workout ID must be greater than 0.");
        }
    }
}
