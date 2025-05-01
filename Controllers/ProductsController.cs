using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CakeProduction.Data;
using CakeProduction.Models;
using CakeProduction.ViewModels;
using System.Text.Json;

namespace CakeProduction.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context,ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;   
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Recipe)
                .ToListAsync()
            );
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Recipe)
                .ThenInclude(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var model = new AddProductViewModel();

            // Get ingredients and convert to SelectList
            var ingredients = await _context.Ingredients
                .Select(i => new
                {
                    i.IngredientId,
                    i.Name,
                    i.UnitOfMeasure
                })
                .ToListAsync();

            // Store in ViewData to be used in JavaScript
            ViewData["IngredientsJson"] = JsonSerializer.Serialize(ingredients);

            // Also populate ViewBag for the dropdown
            ViewBag.Ingredients = new SelectList(ingredients, "IngredientId", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel model)
        {
            ViewBag.Ingredients = await _context.Ingredients
             .Select(i => new SelectListItem
             {
                 Value = i.IngredientId.ToString(),
                 Text = $"{i.Name} ({i.UnitOfMeasure})"
             })
             .ToListAsync();
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach(var ingredientModel in model.Ingredients)
                {
                    if (string.IsNullOrWhiteSpace(ingredientModel.Name))
                    {
                        ModelState.AddModelError("", "All ingredients must have a name");
                        return View(model);
                    }
                    if (ingredientModel.Quantity <= 0)
                    {
                        ModelState.AddModelError("", "All ingredients must have a positive quantity");
                        return View(model);
                    }
                }
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    IsActive = true
                };
                var recipe = new Recipe
                {
                    Instructions = model.Instructions,
                    PreparationTime = model.PreparationTime,
                    YieldQuantity = model.YieldQuantity,
                    YieldUnit = model.YieldUnit,
                    RecipeIngredients = new List<RecipeIngredient>()
                };

                foreach (var ingredientModel in model.Ingredients)
                {
                    var ingredient = await _context.Ingredients
                        .FirstOrDefaultAsync(i => i.IngredientId == ingredientModel.IngredientId) ?? new Ingredient
                        {
                            Name = ingredientModel.Name,
                            UnitOfMeasure = ingredientModel.Unit,
                            CurrentStock = 0,
                            SupplierInfo = "To be determined"
                        };
                    recipe.RecipeIngredients.Add(new RecipeIngredient
                    {
                        Ingredient = ingredient,
                        Quantity = ingredientModel.Quantity
                    });
                }
                product.Recipe = recipe;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
                ViewBag.Ingredients = await _context.Ingredients
                    .Select(i => new SelectListItem
                    {
                        Value = i.IngredientId.ToString(),
                        Text = $"{i.Name} ({i.UnitOfMeasure})"
                    }).ToListAsync();
                return View(model);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,ImageUrl,IsActive")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
