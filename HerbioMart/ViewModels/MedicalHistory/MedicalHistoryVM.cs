namespace HerbioMart.ViewModels.MedicalHistory;

public class MedicalHistoryVM
{
    public int MedicalHistoryId { get; set; }
    public bool Diabetes { get; set; }
    public bool Hypertension { get; set; }
    public bool Asthma { get; set; }
    public bool Smoker { get; set; }
    public bool HeartDisease { get; set; }
    public bool KidneyDisease { get; set; }
    public bool LiverDisease { get; set; }
    public string? OtherNotes { get; set; }
}