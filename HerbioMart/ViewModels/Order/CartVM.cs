namespace HerbioMart.ViewModels.Order;

public class CartVM
{
    public List<CartItemVM> Items { get; set; } = new();
    public decimal ItemsTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal GrandTotal { get; set; }
}