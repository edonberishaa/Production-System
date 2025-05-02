using CakeProduction.Data;
using CakeProduction.Models;
using CakeProduction.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace CakeProduction.Controllers
{
    public class ProductionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductionLogger _productionLogger;

        public ProductionController(ApplicationDbContext context,
              IProductionLogger productionLogger)
        {
            _context = context;
            _productionLogger = productionLogger;

        }

        [HttpGet]
        public async Task<IActionResult> CalculateRequirements(int recipeId,decimal desiredQuantity)
        {
            var recipe = await _context.Recipes
         .Include(r => r.RecipeIngredients)
             .ThenInclude(ri => ri.Ingredient)
         .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null) return NotFound();

            var multiplier = desiredQuantity / recipe.YieldQuantity;
            var results = new List<object>();

            foreach (var ri in recipe.RecipeIngredients)
            {
                var requiredAmount = ri.Quantity * multiplier;
                var status = ri.Ingredient.CurrentStock >= requiredAmount ? "Sufficient" : "Insufficient";

                results.Add(new
                {
                    ingredientName = ri.Ingredient.Name,
                    requiredAmount = Math.Round(requiredAmount, 2),
                    unit = ri.Ingredient.UnitOfMeasure,
                    availableStock = ri.Ingredient.CurrentStock,
                    status
                });
            }
            return Ok(results);
        }
        [HttpGet]
        public async Task<IActionResult> History(
            [FromQuery] int? productId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate
            )
        {
            ViewBag.Products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.Name
                })
                .ToListAsync();

            var history = await _productionLogger.GetProductionHistoryAsync(
              productId,
              fromDate,
              toDate);

            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmProduction([FromBody] ProductionRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var recipe = await _context.Recipes
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefaultAsync(r => r.RecipeId == request.RecipeId);

                if (recipe == null) return NotFound("Recipe not found");

                var multiplier = request.DesiredQuantity / recipe.YieldQuantity;
                var insufficientIngredients = new List<string>();

                // First pass: Check all ingredients
                foreach (var ri in recipe.RecipeIngredients)
                {
                    var requiredAmount = ri.Quantity * multiplier;
                    if (ri.Ingredient.CurrentStock < requiredAmount)
                    {
                        insufficientIngredients.Add($"{ri.Ingredient.Name} (Need: {requiredAmount}, Have: {ri.Ingredient.CurrentStock})");
                    }
                }

                if (insufficientIngredients.Any())
                {
                    return BadRequest(new
                    {
                        Message = "Insufficient stock",
                        Details = insufficientIngredients
                    });
                }

                // Second pass: Deduct ingredients
                foreach (var ri in recipe.RecipeIngredients)
                {
                    var requiredAmount = ri.Quantity * multiplier;
                    ri.Ingredient.CurrentStock -= requiredAmount;
                    _context.Update(ri.Ingredient);
                }

                // Log production
                var productionLog = new ProductionLog
                {
                    RecipeId = recipe.RecipeId,
                    ProductId = recipe.ProductId,
                    QuantityProduced = request.DesiredQuantity,
                    ProductionDate = DateTime.Now,
                    ProducedBy = User?.Identity?.Name ?? "System",
                    Notes = "Torte"

                };
                _context.ProductionLogs.Add(productionLog);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _context.Entry(productionLog).ReloadAsync();
                return Ok(new
                {
                    Message = "Production confirmed successfully",
                    ProductionId = productionLog.ProductionLogId,
                    UpdatedStocks = recipe.RecipeIngredients
                    .Select(ri => new
                    {
                        IngredientId = ri.IngredientId,
                        NewStock = ri.Ingredient.CurrentStock
                    })
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex); // <--- Log this during development
                return StatusCode(500, new { Message = "Production failed", Error = ex.Message });
            }
        }
    }
}
