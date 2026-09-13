namespace HerbioMart.ViewModels.Auth;

public class LoginVM
{
    public string EmailOrUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}