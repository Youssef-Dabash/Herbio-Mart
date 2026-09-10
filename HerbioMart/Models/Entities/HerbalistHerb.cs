namespace HerbioMart.Models.Entities;

public class HerbalistHerb
{
    public int HerbalistId { get; set; }
    public int HerbId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual Herbalist Herbalist { get; set; } = null!;
    public virtual Herb Herb { get; set; } = null!;
}