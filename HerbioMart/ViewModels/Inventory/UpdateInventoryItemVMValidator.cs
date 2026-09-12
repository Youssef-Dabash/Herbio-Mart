using FluentValidation;
using HerbioMart.ViewModels.Inventory;

namespace HerbioMart.Validators.Inventory;

public class UpdateInventoryItemVMValidator : AbstractValidator<UpdateInventoryItemVM>
{
    public UpdateInventoryItemVMValidator()
    {
        RuleFor(x => x.HerbId)
            .GreaterThan(0).WithMessage("Invalid herb reference.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0 EGP.")
            .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10,000 EGP.");
    }
}