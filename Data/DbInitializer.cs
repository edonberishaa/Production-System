using CakeProduction.Data;
using CakeProduction.Models;
using Microsoft.AspNetCore.Identity;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        context.Database.EnsureCreated();

        // Seed admin user
        //if (!userManager.Users.Any())
        //{
        //    var adminUser = new IdentityUser { UserName = "admin@cakes.com", Email = "admin@cakes.com" };
        //    await userManager.CreateAsync(adminUser, "Admin@123");
        //    await userManager.AddToRoleAsync(adminUser, "Admin");
        //}

        // Seed products if empty
        if (!context.Products.Any())
        {
            var ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "Flour", UnitOfMeasure = "kg", CurrentStock = 50 },
                new Ingredient { Name = "Sugar", UnitOfMeasure = "kg", CurrentStock = 30 },
                new Ingredient { Name = "Eggs", UnitOfMeasure = "units", CurrentStock = 200 },
                new Ingredient { Name = "Butter", UnitOfMeasure = "kg", CurrentStock = 20 },
                new Ingredient { Name = "Chocolate", UnitOfMeasure = "kg", CurrentStock = 15 }
            };

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Chocolate Cake",
                    Description = "Rich chocolate cake with buttercream frosting",
                    ImageUrl = "/images/chocolate-cake.jpg",
                    IsActive = true,
                    Recipe = new Recipe
                    {
                        Instructions = "1. Mix dry ingredients\n2. Add wet ingredients\n3. Bake at 350°F for 45 minutes",
                        PreparationTime = 60,
                        YieldQuantity = 1,
                        YieldUnit = "cake",
                        RecipeIngredients = new List<RecipeIngredient>
                        {
                            new RecipeIngredient { Ingredient = ingredients[0], Quantity = 0.5m },
                            new RecipeIngredient { Ingredient = ingredients[1], Quantity = 0.3m },
                            new RecipeIngredient { Ingredient = ingredients[2], Quantity = 4 },
                            new RecipeIngredient { Ingredient = ingredients[3], Quantity = 0.25m },
                            new RecipeIngredient { Ingredient = ingredients[4], Quantity = 0.4m }
                        }
                    }
                }
            };

            context.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}