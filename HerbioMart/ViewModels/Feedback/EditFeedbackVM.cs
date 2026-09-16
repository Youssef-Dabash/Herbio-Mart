using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.Feedback
{
    public class EditFeedbackVM
    {
        [Required]
        public int FeedbackId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "يرجى كتابة التعليق")]
        public string Comment { get; set; } = string.Empty;
    }
}
