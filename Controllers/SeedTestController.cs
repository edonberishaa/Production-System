using CakeProduction.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeedTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            var products = _context.Products
                .Include(p => p.Recipe)
                .ThenInclude(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToList();

            return Ok(products);
        }

        [HttpGet("ingredients")]
        public IActionResult GetIngredients()
        {
            var ingredients = _context.Ingredients.ToList();
            return Ok(ingredients);
        }

        [HttpGet("production-logs")]
        public IActionResult GetProductionLogs()
        {
            var logs = _context.ProductionLogs
                .Include(pl => pl.Product)
                .ToList();

            return Ok(logs);
        }
    }
}