using DevSpot.Constants;
using Microsoft.AspNetCore.Identity;

namespace DevSpot.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManger = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (! await roleManger.RoleExistsAsync(Roles.ADMIN))
            {
                await roleManger.CreateAsync(new IdentityRole(roleName: Roles.ADMIN));
            }

            if (!await roleManger.RoleExistsAsync(Roles.JOB_SEEKER))
            {
                await roleManger.CreateAsync(new IdentityRole(roleName: Roles.JOB_SEEKER));
            }

            if (!await roleManger.RoleExistsAsync(Roles.EMPLOYER))
            {
                await roleManger.CreateAsync(new IdentityRole(roleName: Roles.EMPLOYER));
            }
        }
    }
}
