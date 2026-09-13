using FluentValidation;

namespace HerbioMart.ViewModels.Auth;

public class LoginVMValidator : AbstractValidator<LoginVM>
{
    public LoginVMValidator()
    {
        RuleFor(x => x.EmailOrUserName)
            .NotEmpty().WithMessage("Email or Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}