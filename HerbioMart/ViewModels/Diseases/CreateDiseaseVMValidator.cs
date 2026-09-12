using FluentValidation;

namespace HerbioMart.ViewModels.Diseases;

public class CreateDiseaseVMValidator : AbstractValidator<CreateDiseaseVM>
{
    public CreateDiseaseVMValidator()
    {
        RuleFor(x => x.DiseaseName)
            .NotEmpty().WithMessage("Disease name is required.")
            .MaximumLength(150).WithMessage("Disease name cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description should be at least 10 characters.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Symptoms)
            .NotEmpty().WithMessage("Symptoms are required to help patients match recipes.")
            .MinimumLength(5).WithMessage("Symptoms text is too short.")
            .MaximumLength(1500).WithMessage("Symptoms cannot exceed 1500 characters.");
    }
}