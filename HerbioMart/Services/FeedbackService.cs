using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HerbioMart.Data; 
using HerbioMart.Models; 
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Feedback;

namespace HerbioMart.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task> GetRecipeReviewsAsync(int recipeId)
        {
            return await _context.Feedbacks
                .Where(f => f.RecipeId == recipeId)
                .Select(f => new FeedbackVM
                {
                    Id = f.Id,
                    PatientName = f.Patient != null ? f.Patient.FullName : "patient",
                    Comment = f.Comment,
                    Rating = f.Rating,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task AddReviewAsync(CreateFeedbackVM model, int patientId)
        {
            var feedback = new Feedback
            {
                RecipeId = model.RecipeId,
                PatientId = patientId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.Now
            };

            _context.Feedbacks.Add(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateReviewAsync(EditFeedbackVM model, int patientId)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == model.FeedbackId && f.PatientId == patientId);
            if (feedback == null) return false;

            feedback.Rating = model.Rating;
            feedback.Comment = model.Comment;

            _context.Feedbacks.Update(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DeleteReviewAsync(int feedbackId, int patientId)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == feedbackId && f.PatientId == patientId);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
