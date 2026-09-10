namespace HerbioMart.Models.Entities;

public class MedicalHistory
{
    public int MedicalHistoryId { get; set; }
    public int PatientId { get; set; }
    public bool Diabetes { get; set; }
    public bool Hypertension { get; set; }
    public bool Asthma { get; set; }
    public bool Smoker { get; set; }
    public bool HeartDisease { get; set; }
    public bool KidneyDisease { get; set; }
    public bool LiverDisease { get; set; }
    public string? OtherNotes { get; set; }

    // Navigation Property
    public virtual Patient Patient { get; set; } = null!;
}