namespace HerbioMart.Models.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }
    public int PatientId { get; set; }
    public int RecipeId { get; set; }
    public double RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Recipe Recipe { get; set; } = null!;
}