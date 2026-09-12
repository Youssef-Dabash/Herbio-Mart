using FluentValidation;
using HerbioMart.ViewModels.Inventory;

namespace HerbioMart.Validators.Inventory;

public class AddToInventoryVMValidator : AbstractValidator<AddToInventoryVM>
{
    public AddToInventoryVMValidator()
    {
        RuleFor(x => x.HerbId)
            .GreaterThan(0).WithMessage("Please select an herb from the catalog.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Selling price must be greater than 0 EGP.")
            .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10,000 EGP.");
    }
}
