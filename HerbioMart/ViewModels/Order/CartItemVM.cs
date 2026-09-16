namespace HerbioMart.ViewModels.Order;

public enum CartItemType { Herb, Recipe }

public class CartItemVM
{
    public CartItemType ItemType { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int HerbalistId { get; set; }
    public string HerbalistName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; } // Rate per Kg for herbs or Unit price for recipes
    public int Quantity { get; set; }      // Grams for herbs or Packs for recipes
    public decimal SubTotal { get; set; }
    public List<HerbVendorOptionVM> AvailableVendors { get; set; } = new();
}