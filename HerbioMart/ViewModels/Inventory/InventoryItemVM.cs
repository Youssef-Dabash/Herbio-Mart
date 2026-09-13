namespace HerbioMart.ViewModels.Inventory;

public class InventoryItemVM
{
    public int HerbId { get; set; }
    public string HerbName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}