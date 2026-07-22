using Microsoft.AspNetCore.Identity;
using TechRent.Data;

namespace TechRent.Services
{
    public class IdentitySeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentitySeeder(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            string[] roles = { "Administrador", "Supervisor", "Operador", "Consulta" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await _userManager.FindByEmailAsync("admin@techrent.com") == null)
            {
                var admin = new IdentityUser
                {
                    UserName = "admin@techrent.com",
                    Email = "admin@techrent.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Administrador");
                }
            }
        }
    }
}
