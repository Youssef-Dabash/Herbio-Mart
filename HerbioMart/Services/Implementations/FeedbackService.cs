using Microsoft.EntityFrameworkCore;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _context;

    public FeedbackService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
    {
        return await _context.Feedbacks
            .Include(f => f.Recipe)
            .Include(f => f.Patient)
                .ThenInclude(p => p.User)
            .OrderByDescending(f => f.RatingDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetFeedbacksByPatientUserIdAsync(int userId)
    {
        return await _context.Feedbacks
            .Include(f => f.Recipe)
            .Where(f => f.Patient.UserId == userId)
            .OrderByDescending(f => f.RatingDate)
            .AsNoTracking()
            .ToListAsync();
    }

    // 1. يجهز الفورم: إذا كان مقيمها قبل كده يرجع بياناته للتعديل، وإلا يرجع فورم جديد
    public async Task<FeedbackFormVM?> GetFeedbackFormAsync(int recipeId, int userId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null) return null;

        var existingFeedback = await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.Patient.UserId == userId);

        if (existingFeedback != null)
        {
            return new FeedbackFormVM
            {
                FeedbackId = existingFeedback.FeedbackId,
                RecipeId = recipe.RecipeId,
                RecipeName = recipe.RecipeName,
                RatingValue = existingFeedback.RatingValue,
                Comment = existingFeedback.Comment
            };
        }

        return new FeedbackFormVM
        {
            FeedbackId = 0,
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            RatingValue = 5.0
        };
    }

    // 2. الـ Upsert: تعديل نفس الـ Row أو إضافة سطر جديد
    public async Task<bool> SaveFeedbackAsync(FeedbackFormVM model, int userId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        if (patient == null) return false;

        // البحث عن التقييم الحالي للمريض على نفس الوصفة
        var feedback = await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.RecipeId == model.RecipeId && f.PatientId == patient.PatientId);

        if (feedback != null)
        {
            // تعديل نفس الـ Row وتحديث تاريخ التعديل
            feedback.RatingValue = model.RatingValue;
            feedback.Comment = model.Comment;
            feedback.RatingDate = DateTime.UtcNow;
        }
        else
        {
            // إضافة تقييم جديد
            feedback = new Feedback
            {
                PatientId = patient.PatientId,
                RecipeId = model.RecipeId,
                RatingValue = model.RatingValue,
                Comment = model.Comment,
                RatingDate = DateTime.UtcNow
            };
            _context.Feedbacks.Add(feedback);
        }

        await _context.SaveChangesAsync();

        // إعادة حساب المتوسط بدقة رقم عشري واحد وتحديث العدد الإجمالي
        await RecalculateRecipeStatsAsync(model.RecipeId);
        return true;
    }

    public async Task<bool> DeleteFeedbackAsync(int feedbackId, int userId)
    {
        var feedback = await _context.Feedbacks
            .Include(f => f.Patient)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId && f.Patient.UserId == userId);

        if (feedback == null) return false;

        int recipeId = feedback.RecipeId;
        _context.Feedbacks.Remove(feedback);
        await _context.SaveChangesAsync();

        await RecalculateRecipeStatsAsync(recipeId);
        return true;
    }

    // الحسبة برقم عشري واحد
    private async Task RecalculateRecipeStatsAsync(int recipeId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null) return;

        var ratings = await _context.Feedbacks
            .Where(f => f.RecipeId == recipeId)
            .Select(f => f.RatingValue)
            .ToListAsync();

        if (ratings.Any())
        {
            recipe.TotalRatings = ratings.Count;
            recipe.AverageRating = Math.Round(ratings.Average(), 1);
        }
        else
        {
            recipe.TotalRatings = 0;
            recipe.AverageRating = 0.0;
        }

        await _context.SaveChangesAsync();
    }
}