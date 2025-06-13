using CakeProduction.Models;

namespace CakeProduction.ViewModels
{
    public class UserPrefencesViewModel
    {
        public List<Ingredient> AllIngredients { get; set; } = new List<Ingredient>();
        public List<int> SelectedIngredients { get; set; } = new List<int>();

    }
}
