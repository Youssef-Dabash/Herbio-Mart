
using HerbioMart.Models.Enums;

namespace HerbioMart.ViewModels.Herbalist
{
    public class HerbalistSubOrderListItemVM
    {
        public int SubOrderId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int TotalItemsCount { get; set; }
        public decimal SubTotal { get; set; }
        public SubOrderStatus Status { get; set; }
    }

    public class SubOrderItemDetailVM
    {
        public string ItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty; // "Herb" or "Recipe"
        public int Quantity { get; set; }                     // Grams or Units
        public string UnitLabel { get; set; } = string.Empty; // "g" or "unit(s)"
        public decimal UnitPrice { get; set; }                // Price per Kg or per unit
        public decimal SubTotal { get; set; }
    }

    public class HerbalistSubOrderDetailsVM
    {
        public int SubOrderId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public SubOrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }

        // Patient & Delivery Information
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        // Package Items
        public List<SubOrderItemDetailVM> Items { get; set; } = new();
    }
}