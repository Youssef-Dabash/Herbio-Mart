using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.Feedback;

public class FeedbackFormVM
{
    public int FeedbackId { get; set; }
    public int RecipeId { get; set; }
    public string? RecipeName { get; set; }
    public double RatingValue { get; set; } = 5.0;
    public string? Comment { get; set; }
}