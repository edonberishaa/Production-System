using CakeProduction.Data;
using CakeProduction.Models;
using CakeProduction.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeProduction.Controllers
{
    public class RecipeSuggestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AIRecipeSuggestionService _aiService;

        public RecipeSuggestionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager,AIRecipeSuggestionService aiService)
        {
            _context = context;
            _userManager = userManager;
            _aiService = aiService;
        }

        public async Task<IActionResult> AISuggestions()
        {
            var user = await _userManager.GetUserAsync(User);
            var preferredIngredientIds = await _context.UserPreferences
                .Where(up => up.UserId == user.Id)
                .Select(up => up.IngredientId)
                .ToListAsync();

            var ingredients = await _context.Ingredients
                .Where(i => preferredIngredientIds.Contains(i.IngredientId))
                .Select(i => i.Name)
                .ToListAsync();

            var aiResponse = await _aiService.GetAIResponse(ingredients);
            ViewBag.AISuggestions = aiResponse;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AISuggestionsPartial");
            }

            return View();
        }
    }
}
