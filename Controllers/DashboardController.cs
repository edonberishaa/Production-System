using CakeProduction.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CakeProduction.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var turnoverPerDay = _context.ProductionLogs
                .GroupBy(p => p.ProductionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalProduced = g.Sum(p => p.QuantityProduced)
                }).ToList();

            var mostUsedRecipes = _context.ProductionLogs
                .GroupBy(p => p.RecipeId)
                .Select(g => new
                {
                    RecipeName = g.First().Recipe.Product.Name,
                    TimesUsed = g.Count()
                }).OrderByDescending(p => p.TimesUsed)
                .Take(5)
                .ToList();

            ViewBag.Turnover = turnoverPerDay;
            ViewBag.MostUsed = mostUsedRecipes;

            return View();
        }
    }
}
