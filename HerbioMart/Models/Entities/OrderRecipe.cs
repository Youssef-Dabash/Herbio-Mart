namespace HerbioMart.Models.Entities;

public class OrderRecipe
{
    public int OrderRecipeId { get; set; }
    public int SubOrderId { get; set; }
    public int RecipeId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }

    // Navigation Properties
    public virtual SubOrder SubOrder { get; set; } = null!;
    public virtual Recipe Recipe { get; set; } = null!;
}