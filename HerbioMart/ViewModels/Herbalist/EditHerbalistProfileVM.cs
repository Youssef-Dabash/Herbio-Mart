using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.Herbalist;

public class EditHerbalistProfileVM
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "License number is required")]
    public string LicenseNumber { get; set; } = string.Empty;

    public string Governorate { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;

    public string? Bio { get; set; }
    public string? ExistingImageUrl { get; set; }

    public IFormFile? ProfileImage { get; set; }
}