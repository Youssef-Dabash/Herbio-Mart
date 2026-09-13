namespace HerbioMart.ViewModels.Shop;

public class ShopHerbItemVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = "/img/fruite-item-1.jpg";
    public string? Indication { get; set; } // Benefits
    public string? Description { get; set; }
}