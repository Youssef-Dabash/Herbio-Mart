namespace HerbioMart.ViewModels.Herbalist;

public class CreateHerbVM
{
    public string HerbName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Warnings { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; }
}