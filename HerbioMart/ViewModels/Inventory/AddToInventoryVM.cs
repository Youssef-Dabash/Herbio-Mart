using Microsoft.AspNetCore.Mvc.Rendering;

namespace HerbioMart.ViewModels.Inventory;

public class AddToInventoryVM
{
    public int HerbId { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> AvailableCatalogHerbs { get; set; } = new();
}