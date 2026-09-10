using HerbioMart.Models.Enums;

namespace HerbioMart.Models.Entities;

public class Patient
{
    public int PatientId { get; set; }
    public int UserId { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual MedicalHistory? MedicalHistory { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}