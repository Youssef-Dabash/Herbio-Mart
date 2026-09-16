using FluentValidation;

namespace HerbioMart.ViewModels.Order
{
    public class CheckoutValidator : AbstractValidator<CheckoutVM>
    {
        public CheckoutValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Delivery destination address is required.")
                .MinimumLength(10).WithMessage("Please enter a detailed delivery address (minimum 10 characters).")
                .MaximumLength(300).WithMessage("Shipping address cannot exceed 300 characters.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Please choose a payment method.")
                .Must(m => m == "CashOnDelivery" || m == "OnlineCard")
                .WithMessage("Selected payment method is invalid.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Delivery notes cannot exceed 500 characters.");
        }
    }
}