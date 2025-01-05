using Microsoft.AspNetCore.Identity;
using DevSpot.Constants;
using System.Runtime.InteropServices;
namespace DevSpot.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager, email: "admin@devspot.com", pwd: "Admin1234@", role: Roles.ADMIN);
            await CreateUserWithRole(userManager, email: "jobseeker@devspot.com", pwd: "Jobseeker1234@", role: Roles.JOB_SEEKER);
            await CreateUserWithRole(userManager, email: "employer@devspot.com", pwd: "Employer1234@", role: Roles.EMPLOYER);
        }

        private static async Task CreateUserWithRole(UserManager<IdentityUser> userManager, string email, string pwd, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                };

                var result = await userManager.CreateAsync(user, password: pwd);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role: Roles.ADMIN);
                }
                else
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    throw new Exception($"Failed to create user with email {user.Email}. Errors: {errorMessages}");
                }
            }
        }
    }
}
