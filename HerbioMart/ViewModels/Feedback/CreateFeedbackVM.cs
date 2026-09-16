using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.Feedback
{
    public class CreateFeedbackVM
    {
        [Required]
        public int RecipeId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "must between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "pls write a comment")]
        public string Comment { get; set; } = string.Empty;
    }
}
