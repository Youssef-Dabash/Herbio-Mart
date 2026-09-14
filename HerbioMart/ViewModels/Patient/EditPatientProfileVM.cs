using System;
using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.Patient;

public class EditPatientProfileVM
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? ExistingImageUrl { get; set; }
    public IFormFile? ProfileImage { get; set; }
}