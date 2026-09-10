namespace HerbioMart.Models.Entities;

public class Herbalist
{
    public int HerbalistId { get; set; }
    public int UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Bio { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Herb> AddedHerbs { get; set; } = new List<Herb>();
    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public virtual ICollection<HerbalistHerb> HerbalistHerbs { get; set; } = new List<HerbalistHerb>();
    public virtual ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
}