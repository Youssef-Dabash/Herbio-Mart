using HerbioMart.Models.Enums;

namespace HerbioMart.Models.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int PatientId { get; set; }
    public decimal ItemsTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalPrice { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.New;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
}