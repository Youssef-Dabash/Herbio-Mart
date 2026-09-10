namespace HerbioMart.Models.Entities;

public class Recipe
{
    public int RecipeId { get; set; }
    public int HerbalistId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public decimal Price { get; set; }
    public double AverageRating { get; set; } = 0.0;
    public int TotalRatings { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Herbalist Herbalist { get; set; } = null!;
    public virtual ICollection<RecipeHerb> RecipeHerbs { get; set; } = new List<RecipeHerb>();
    public virtual ICollection<RecipeDisease> RecipeDiseases { get; set; } = new List<RecipeDisease>();
    public virtual ICollection<OrderRecipe> OrderRecipes { get; set; } = new List<OrderRecipe>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}