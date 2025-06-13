using Microsoft.AspNetCore.Identity;

namespace CakeProduction.Models
{
    public class UserPreference
    {
        public int PreferenceId { get; set; }
        public string UserId { get; set; }
        public int IngredientId { get; set; }

        public virtual IdentityUser User { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
