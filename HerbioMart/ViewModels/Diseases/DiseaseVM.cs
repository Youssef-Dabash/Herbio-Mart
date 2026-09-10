namespace HerbioMart.ViewModels.Diseases;

public class DiseaseVM
{
    public int DiseaseId { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Symptoms { get; set; } = string.Empty;
}