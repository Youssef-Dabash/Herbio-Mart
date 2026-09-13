namespace HerbioMart.ViewModels.Herb;

public class HerbDetailsVM
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Warnings { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public string AddedByHerbalistName { get; set; } = string.Empty;
}