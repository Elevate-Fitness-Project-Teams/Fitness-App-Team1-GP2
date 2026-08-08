using FluentValidation;
using ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Request;

namespace ProgressTrackingService.Features.Progress.WeightHistory.LogWeightHistory.Validator;

public class LogWeightValidator : AbstractValidator<LogWeightRequest>
{
    public LogWeightValidator()
    {
        RuleFor(x => x.Weight)
            .GreaterThan(40)
            .LessThan(200)
            .WithMessage("Weight must be between 40 and 200");
            
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThan(DateTimeOffset.Now)
            .WithMessage("Date is not a valid date format");
        
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}