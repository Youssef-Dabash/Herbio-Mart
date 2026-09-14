using HerbioMart.ViewModels.MedicalHistory;
using System;

namespace HerbioMart.ViewModels.Patient
{
    public class PatientProfileVM
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty; 
        public MedicalHistoryVM? MedicalHistory { get; set; }
    }
}