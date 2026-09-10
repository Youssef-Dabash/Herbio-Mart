using HerbioMart.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HerbioMart.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Herbalist> Herbalists { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<Herb> Herbs { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeHerb> RecipeHerbs { get; set; }
        public DbSet<RecipeDisease> RecipeDiseases { get; set; }
        public DbSet<HerbalistHerb> HerbalistHerbs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SubOrder> SubOrders { get; set; }
        public DbSet<OrderHerb> OrderHerbs { get; set; }
        public DbSet<OrderRecipe> OrderRecipes { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}