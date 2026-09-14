using FluentValidation;

namespace HerbioMart.ViewModels.Feedback;

public class FeedbackFormValidator : AbstractValidator<FeedbackFormVM>
{
    public FeedbackFormValidator()
    {
        RuleFor(x => x.RecipeId)
            .GreaterThan(0)
            .WithMessage("A valid recipe must be selected.");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(1.0, 5.0)
            .WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}