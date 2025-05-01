using CakeProduction.Data;
using CakeProduction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm,string sortOrder)
        {
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["StockSort"] = sortOrder == "stock" ? "stock_desc" : "stock";

            var ingredients = from i in _context.Ingredients select i;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                ingredients = ingredients.Where(i => i.Name.Contains(searchTerm));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    ingredients = ingredients.OrderByDescending(i => i.Name);
                    break;
                case "stock":
                    ingredients = ingredients.OrderBy(i => i.CurrentStock);
                    break;
                case "stock_desc":
                    ingredients = ingredients.OrderByDescending(i => i.CurrentStock);
                    break;
                default:
                    ingredients = ingredients.OrderBy(i => i.Name);
                    break;
            }
            return View(await ingredients.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UnitOfMeasure,CurrentStock,MinimumStockLevel,SupplierInfo")] Ingredient ingredient)
        {
            bool ingredientExists = await _context.Ingredients.AnyAsync(i => i.Name.ToLower() == ingredient.Name.ToLower());

            if(ingredientExists)
            {
                ModelState.AddModelError("Name", $"Ingredient '{ingredient.Name}' already exists. Please update its stock instead.");
                return View(ingredient);
            }

            if (!ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IngredientId,Name,UnitOfMeasure,CurrentStock,MinimumStockLevel,SupplierInfo")] Ingredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.IngredientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }
        public async Task<IActionResult> StockUpdate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockUpdate(int id, decimal adjustment, string operation)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            if (operation == "add")
            {
                ingredient.CurrentStock += adjustment;
            }
            else if (operation == "subtract")
            {
                if (ingredient.CurrentStock < adjustment)
                {
                    ModelState.AddModelError("", $"Cannot subtract more than current stock ({ingredient.CurrentStock} {ingredient.UnitOfMeasure})");
                    return View(ingredient);
                }
                ingredient.CurrentStock -= adjustment;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.IngredientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(ingredient);
        }
        // GET: Ingredients/LowStock
        public async Task<IActionResult> LowStock()
        {
            var lowStockIngredients = await _context.Ingredients
                .Where(i => i.CurrentStock < i.MinimumStockLevel)
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(lowStockIngredients);
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Search(string term)
        {
            var ingredients = await _context.Ingredients
                .Where(i => i.Name.Contains(term))
                .Select(i => new { id = i.IngredientId, text = i.Name })
                .ToListAsync();

            return Json(ingredients);
        }
        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.IngredientId == id);
        }
    }
}