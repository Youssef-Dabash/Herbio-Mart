namespace HerbioMart.Models.Entities;

public class RecipeHerb
{
    public int RecipeHerbId { get; set; }
    public int RecipeId { get; set; }
    public int HerbId { get; set; }
    public double Quantity { get; set; }

    // Navigation Properties
    public virtual Recipe Recipe { get; set; } = null!;
    public virtual Herb Herb { get; set; } = null!;
}