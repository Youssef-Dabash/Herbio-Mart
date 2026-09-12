using FluentValidation;
using HerbioMart.ViewModels.Recipes;

namespace HerbioMart.Validators.Recipes;

public class CreateRecipeVMValidator : AbstractValidator<CreateRecipeVM>
{
    public CreateRecipeVMValidator()
    {
        RuleFor(x => x.RecipeName)
            .NotEmpty().WithMessage("Recipe formulation name is required.")
            .MaximumLength(150).WithMessage("Recipe name cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Recipe description and medical purpose are required.")
            .MinimumLength(15).WithMessage("Description must be at least 15 characters long.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Preparation and administration instructions are required.")
            .MinimumLength(15).WithMessage("Instructions must be at least 15 characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Formulation price must be greater than 0 EGP.")
            .LessThanOrEqualTo(20000).WithMessage("Price cannot exceed 20,000 EGP.");

        RuleFor(x => x.SelectedDiseaseIds)
            .Must(ids => ids != null && ids.Any(id => id > 0))
            .WithMessage("Please select at least one valid targeted health condition.");
        RuleFor(x => x.Herbs)
            .NotEmpty().WithMessage("At least one botanical ingredient must be added to this recipe.");

        RuleForEach(x => x.Herbs).ChildRules(herb =>
        {
            herb.RuleFor(h => h.HerbId)
                .GreaterThan(0).WithMessage("Please select a valid herb specimen from the dropdown.");

            herb.RuleFor(h => h.Quantity)
                .GreaterThan(0).WithMessage("Ingredient weight must be greater than 0 grams.");
        });
    }
}