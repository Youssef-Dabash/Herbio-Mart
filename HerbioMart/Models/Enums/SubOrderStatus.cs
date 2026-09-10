namespace HerbioMart.Models.Enums;

// دورة حياة تجهيز الطلب لدى كل عطار
public enum SubOrderStatus
{
    Pending = 1,
    Accepted = 2,
    Packing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}