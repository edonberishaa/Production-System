using Microsoft.AspNetCore.Identity;

namespace CakeProduction.Services
{
    public class RoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task EnsureRolesCreated()
        {
            string[] roleNames = { "Admin", "ProductionManager", "Baker", "Procurement" };

            foreach(var roleName in roleNames)
            {
                var normalizedRoleName = roleName.ToUpper();
                var roleExist = await _roleManager.RoleExistsAsync(normalizedRoleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(normalizedRoleName));
                }
            }
        }
        public async Task AssignRole(string email,string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null && !await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
