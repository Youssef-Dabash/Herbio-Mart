namespace HerbioMart.ViewModels.Feedback
{
    public class FeedbackVM
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
