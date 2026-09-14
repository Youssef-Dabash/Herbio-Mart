namespace HerbioMart.ViewModels.Herbalist;

public class HerbalistDashboardVM
{
    public int HerbalistId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Bio { get; set; }

    // Real Database Counts
    public int AddedHerbsCount { get; set; }
    public int FormulatedRecipesCount { get; set; }
    public int PendingOrdersCount { get; set; }
}