namespace HerbioMart.ViewModels.Shop;

public class ShopHerbVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    // أضف هذا السطر لحل الخطأ فوراً
    public List<HerbVendorOptionVM> AvailableVendors { get; set; } = new();
}
