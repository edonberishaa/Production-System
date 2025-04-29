using CakeProduction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CakeProduction.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Only seed if database is empty
            if (await context.Products.AnyAsync()) return;

            // Ingredients
            var flour = new Ingredient { Name = "Flour", UnitOfMeasure = "kg", CurrentStock = 100 };
            var sugar = new Ingredient { Name = "Sugar", UnitOfMeasure = "kg", CurrentStock = 50 };
            var eggs = new Ingredient { Name = "Eggs", UnitOfMeasure = "units", CurrentStock = 200 };
            var butter = new Ingredient { Name = "Butter", UnitOfMeasure = "kg", CurrentStock = 30 };
            var chocolate = new Ingredient { Name = "Chocolate", UnitOfMeasure = "kg", CurrentStock = 25 };
            var milk = new Ingredient { Name = "Milk", UnitOfMeasure = "liters", CurrentStock = 40 };
            var vanilla = new Ingredient { Name = "Vanilla Extract", UnitOfMeasure = "ml", CurrentStock = 1000 };

            await context.Ingredients.AddRangeAsync(flour, sugar, eggs, butter, chocolate, milk, vanilla);
            await context.SaveChangesAsync();

            // Products with Recipes
            var chocolateCake = new Product
            {
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake with chocolate frosting",
                ImageUrl = "/images/chocolate-cake.jpg",
                IsActive = true,
                Recipe = new Recipe
                {
                    Instructions = "1. Preheat oven to 350°F (175°C)\n" +
                                 "2. Mix dry ingredients\n" +
                                 "3. Cream butter and sugar\n" +
                                 "4. Add eggs one at a time\n" +
                                 "5. Alternately add dry ingredients and milk\n" +
                                 "6. Bake for 35-40 minutes",
                    PreparationTime = 60,
                    YieldQuantity = 1,
                    YieldUnit = "cake",
                    RecipeIngredients = new[]
                    {
                        new RecipeIngredient { Ingredient = flour, Quantity = 0.5m },
                        new RecipeIngredient { Ingredient = sugar, Quantity = 0.3m },
                        new RecipeIngredient { Ingredient = eggs, Quantity = 4 },
                        new RecipeIngredient { Ingredient = butter, Quantity = 0.25m },
                        new RecipeIngredient { Ingredient = chocolate, Quantity = 0.4m },
                        new RecipeIngredient { Ingredient = milk, Quantity = 0.3m },
                        new RecipeIngredient { Ingredient = vanilla, Quantity = 10 }
                    }
                }
            };

            var vanillaCake = new Product
            {
                Name = "Vanilla Cake",
                Description = "Classic vanilla cake with vanilla buttercream",
                ImageUrl = "/images/vanilla-cake.jpg",
                IsActive = true,
                Recipe = new Recipe
                {
                    Instructions = "1. Preheat oven to 350°F (175°C)\n" +
                                 "2. Cream butter and sugar until light\n" +
                                 "3. Add eggs one at a time\n" +
                                 "4. Mix in vanilla\n" +
                                 "5. Alternately add flour and milk\n" +
                                 "6. Bake for 30-35 minutes",
                    PreparationTime = 50,
                    YieldQuantity = 1,
                    YieldUnit = "cake",
                    RecipeIngredients = new[]
                    {
                        new RecipeIngredient { Ingredient = flour, Quantity = 0.45m },
                        new RecipeIngredient { Ingredient = sugar, Quantity = 0.35m },
                        new RecipeIngredient { Ingredient = eggs, Quantity = 3 },
                        new RecipeIngredient { Ingredient = butter, Quantity = 0.3m },
                        new RecipeIngredient { Ingredient = milk, Quantity = 0.35m },
                        new RecipeIngredient { Ingredient = vanilla, Quantity = 15 }
                    }
                }
            };

            await context.Products.AddRangeAsync(chocolateCake, vanillaCake);
            await context.SaveChangesAsync();

            // Sample production logs
            await context.ProductionLogs.AddRangeAsync(
                new ProductionLog
                {
                    ProductId = chocolateCake.ProductId,
                    RecipeId = chocolateCake.Recipe.RecipeId,
                    QuantityProduced = 5,
                    ProductionDate = DateTime.UtcNow.AddDays(-2),
                    ProducedBy = "System",
                    Notes = "Initial batch"
                },
                new ProductionLog
                {
                    ProductId = vanillaCake.ProductId,
                    RecipeId = vanillaCake.Recipe.RecipeId,
                    QuantityProduced = 3,
                    ProductionDate = DateTime.UtcNow.AddDays(-1),
                    ProducedBy = "System",
                    Notes = "Test production"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}