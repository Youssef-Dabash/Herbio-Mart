
using FluentValidation;
using HerbioMart.ViewModels.Patient;

namespace HerbioMart.ViewModels.PatientProfile;

public class EditPatientProfileValidator : AbstractValidator<EditPatientProfileVM>
{
    public EditPatientProfileValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^01[0125][0-9]{8}$").WithMessage("Please enter a valid Egyptian phone number (e.g. 01012345678).");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || g == "Male" || g == "Female")
            .WithMessage("Please select a valid gender (Male or Female).");
    }
}