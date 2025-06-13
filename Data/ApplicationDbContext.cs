using CakeProduction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<ProductionLog> ProductionLogs { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            builder.Entity<ProductionLog>()
                .HasOne(pl => pl.Recipe)
                .WithMany()
                .HasForeignKey(pl => pl.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProductionLog>()
                .HasIndex(pl => pl.ProductionDate);

            builder.Entity<ProductionLog>()
                .HasIndex(pl => pl.ProductId);

            builder.Entity<Ingredient>(entity =>
            {
                entity.Property(e => e.CurrentStock).HasPrecision(18, 4);
                entity.Property(e => e.MinimumStockLevel).HasPrecision(18, 4);
            });

            builder.Entity<ProductionLog>(entity =>
            {
                entity.Property(e => e.QuantityProduced).HasPrecision(18, 4);
            });

            builder.Entity<RecipeIngredient>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 4);
            });
            builder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(up => up.PreferenceId);
                entity.HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .IsRequired();

                entity.HasOne(up => up.Ingredient)
                .WithMany()
                .HasForeignKey(up => up.IngredientId)
                .IsRequired();
            });
        }
    }
}
