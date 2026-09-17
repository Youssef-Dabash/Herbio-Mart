namespace HerbioMart.ViewModels.Shop;

public class ShopRecipeVM
{
    public int Id { get; set; }
    public int HerbalistId { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string HerbalistName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int HerbsCount { get; set; }
    public List<string> DiseasesTreated { get; set; } = new();
}
