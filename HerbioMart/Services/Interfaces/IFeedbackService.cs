using System.Collections.Generic;
using System.Threading.Tasks;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task> GetRecipeReviewsAsync(int recipeId);
        Task AddReviewAsync(CreateFeedbackVM model, int patientId);
        Task UpdateReviewAsync(EditFeedbackVM model, int patientId);
        Task DeleteReviewAsync(int feedbackId, int patientId);
    }
}
