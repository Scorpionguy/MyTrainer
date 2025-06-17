using Microsoft.AspNetCore.Identity;
using MyTrainer.Models;

namespace MyTrainer.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Создание пользователя с ролью
            var adminEmail = "lapinfedor11@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
                var result = await userManager.AddToRoleAsync(adminUser, "Admin");

                if (result.Succeeded)
                    Console.WriteLine("Пользователь назначен админом.");
                else
                    foreach (var error in result.Errors)
                        Console.WriteLine(error.Description);
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }
    }
}
