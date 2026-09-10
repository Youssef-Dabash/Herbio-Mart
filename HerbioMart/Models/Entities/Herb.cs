namespace HerbioMart.Models.Entities;

public class Herb
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Warnings { get; set; } = string.Empty;
    public string? ImageURL { get; set; }

    public int? AddedByHerbalistId { get; set; }

    // Navigation Properties
    public virtual Herbalist? AddedByHerbalist { get; set; }
    public virtual ICollection<HerbalistHerb> HerbalistHerbs { get; set; } = new List<HerbalistHerb>();
    public virtual ICollection<RecipeHerb> RecipeHerbs { get; set; } = new List<RecipeHerb>();
    public virtual ICollection<OrderHerb> OrderHerbs { get; set; } = new List<OrderHerb>();
}