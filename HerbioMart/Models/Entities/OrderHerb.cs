namespace HerbioMart.Models.Entities;

public class OrderHerb
{
    public int OrderHerbId { get; set; }
    public int SubOrderId { get; set; }
    public int HerbId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation Properties
    public virtual SubOrder SubOrder { get; set; } = null!;
    public virtual Herb Herb { get; set; } = null!;
}