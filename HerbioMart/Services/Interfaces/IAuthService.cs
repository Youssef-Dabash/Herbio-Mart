using HerbioMart.Models.Entities;
using HerbioMart.ViewModels.Auth;

namespace HerbioMart.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterVM model);
    Task<(bool Success, string? Error, User? User)> ValidateUserAsync(LoginVM model);
}