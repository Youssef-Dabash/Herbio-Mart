using FluentValidation;

namespace HerbioMart.ViewModels.Herbalist;

public class CreateHerbVMValidator : AbstractValidator<CreateHerbVM>
{
    public CreateHerbVMValidator()
    {
        RuleFor(x => x.HerbName)
            .NotEmpty().WithMessage("Common herb name is required.")
            .MaximumLength(100).WithMessage("Herb name cannot exceed 100 characters.");

        RuleFor(x => x.ScientificName)
            .NotEmpty().WithMessage("Scientific botanical name is required.")
            .MaximumLength(150).WithMessage("Scientific name cannot exceed 150 characters.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Scientific name must contain only Latin characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Botanical description is required.")
            .MinimumLength(15).WithMessage("Description must be at least 15 characters long.");

        RuleFor(x => x.Benefits)
            .NotEmpty().WithMessage("Medical therapeutic benefits are required.");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage and safe usage instructions are required.");

        RuleFor(x => x.Warnings)
            .NotEmpty().WithMessage("Safety warnings and contraindications are required.");

        When(x => x.ImageFile != null, () =>
        {
            RuleFor(x => x.ImageFile!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("Image file size cannot exceed 5 MB.");

            RuleFor(x => x.ImageFile!.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage("Only JPEG, PNG, or WebP image formats are permitted.");
        });
    }
}