using AutoMapper;
using HerbioMart.Data;
using HerbioMart.Models.Entities;
using HerbioMart.Models.Enums;
using HerbioMart.Services.Interfaces;
using HerbioMart.ViewModels.Auth;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterVM model)
    {
        var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());
        if (emailExists) return (false, "This email address is already registered.", null);

        var usernameExists = await _context.Users.AnyAsync(u => u.UserName.ToLower() == model.UserName.Trim().ToLower());
        if (usernameExists) return (false, "This username is already taken.", null);

        var user = _mapper.Map<User>(model);
        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        if (model.Role == UserRole.Herbalist)
        {
            var firstName = model.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Herbalist";
            var randomCode = Guid.NewGuid().ToString("N")[..6].ToUpper();

            user.Herbalist = new Herbalist
            {
                LicenseNumber = $"{firstName}-{randomCode}",
                Bio = null
            };
        }
        else
        {
            user.Patient = new Patient
            {
                BirthDate = DateTime.Today,
                Gender = default
            };
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, null, user);
    }

    public async Task<(bool Success, string? Error, User? User)> ValidateUserAsync(LoginVM model)
    {
        var cleanInput = model.EmailOrUserName.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == cleanInput || u.UserName.ToLower() == cleanInput);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return (false, "Invalid email/username or password.", null);
        }

        return (true, null, user);
    }
}