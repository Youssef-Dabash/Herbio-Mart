using HerbioMart.Data;
using HerbioMart.Services.Implementations;
using HerbioMart.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HerbioMart;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
        builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IDiseaseService, DiseaseService>();
        builder.Services.AddScoped<IHerbService, HerbService>();
        builder.Services.AddScoped<IRecipeService, RecipeService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); 
        }

        app.UseHttpsRedirection();
        app.UseRouting(); 

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
