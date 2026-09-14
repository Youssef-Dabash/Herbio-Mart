using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Services.Interfaces;

public interface IFeedbackService
{
    Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
    Task<IEnumerable<Feedback>> GetFeedbacksByPatientUserIdAsync(int userId);
    Task<FeedbackFormVM?> GetFeedbackFormAsync(int recipeId, int userId);
    Task<bool> SaveFeedbackAsync(FeedbackFormVM model, int userId);
    Task<bool> DeleteFeedbackAsync(int feedbackId, int userId);
}