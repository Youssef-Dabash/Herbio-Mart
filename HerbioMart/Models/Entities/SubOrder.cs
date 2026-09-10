using HerbioMart.Models.Enums;

namespace HerbioMart.Models.Entities;

public class SubOrder
{
    public int SubOrderId { get; set; }
    public int OrderId { get; set; }
    public int HerbalistId { get; set; }
    public decimal SubTotal { get; set; }
    public SubOrderStatus Status { get; set; } = SubOrderStatus.Pending;

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
    public virtual Herbalist Herbalist { get; set; } = null!;
    public virtual ICollection<OrderHerb> OrderHerbs { get; set; } = new List<OrderHerb>();
    public virtual ICollection<OrderRecipe> OrderRecipes { get; set; } = new List<OrderRecipe>();
}