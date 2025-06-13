using CakeProduction.Data;
using CakeProduction.Models;
using CakeProduction.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Controllers
{
    public class UserPreferencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserPreferencesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserPreferences/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var allIngredients = await _context.Ingredients.ToListAsync();

            var selectedIngredientIds = await _context.UserPreferences
                .Where(up => up.UserId == user.Id)
                .Select(up => up.IngredientId)
                .ToListAsync();

            var model = new UserPrefencesViewModel
            {
                AllIngredients = allIngredients,
                SelectedIngredients = selectedIngredientIds
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserPrefencesViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var existingPrefs = _context.UserPreferences.Where(up => up.UserId == user.Id);
                _context.UserPreferences.RemoveRange(existingPrefs);

                if (model.SelectedIngredients != null)
                {
                    foreach (var ingredientId in model.SelectedIngredients)
                    {
                        _context.UserPreferences.Add(new UserPreference
                        {
                            UserId = user.Id,
                            IngredientId = ingredientId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            // If we got here, reload all ingredients for view
            model.AllIngredients = await _context.Ingredients.ToListAsync();
            return View(model);
        }
    }
}
