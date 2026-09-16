namespace HerbioMart.ViewModels.Order
{
    public class CheckoutVM
    {
        // Loaded automatically from authenticated profile for display
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Order-specific delivery input
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = "CashOnDelivery";

        // Dynamic Cart Summary
        public CartVM Cart { get; set; } = new();

        public List<CartItemVM> Items => Cart.Items;
        public decimal DeliveryFee => Cart.DeliveryFee;
        public decimal ItemsTotal => Cart.ItemsTotal;
        public decimal GrandTotal => Cart.GrandTotal;
    }
}