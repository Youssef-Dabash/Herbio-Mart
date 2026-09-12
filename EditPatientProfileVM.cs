using System;
using System.ComponentModel.DataAnnotations;

namespace HerbioMart.ViewModels.PatientProfile
{
    public class EditPatientProfileVM
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Incorrect Phone Number")]
        public string Phone { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; } = string.Empty;
    }
}