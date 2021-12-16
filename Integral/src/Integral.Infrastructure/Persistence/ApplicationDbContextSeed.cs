using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Security;
using Integral.Application.Common.Services;
using Integral.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Integral.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            await CreateDefaultUser(serviceProvider);

            await CreateDefaultApplicationSettings(serviceProvider);
        }

        private static async Task CreateDefaultUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var username = "";
            var password = "";

            var user = new ApplicationUser
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
            };

            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new ApplicationException("Default user creation failed.");
                }
            }
            else
            {
                user = existingUser;
            }

            var claims = await userManager.GetClaimsAsync(user);
            if (!claims.Any(m => m.Type == ClaimTypes.Role))
            {
                var adminRoleClaim = new Claim(ClaimTypes.Role, Role.Admin);

                await userManager.AddClaimAsync(user, adminRoleClaim);
            }
        }

        private static async Task CreateDefaultApplicationSettings(IServiceProvider serviceProvider)
        {
            var applicationSettingService = serviceProvider.GetRequiredService<IApplicationSettingService>();

            var commonSettings = await applicationSettingService.GetAsync<CommonSettingsDTO>();
            if (commonSettings == null)
            {
                await applicationSettingService.SetAsync(new CommonSettingsDTO());
            }

            var landingCarouselSettings = await applicationSettingService.GetAsync<LandingCarouselSettingsDTO>();
            if (landingCarouselSettings == null)
            {
                await applicationSettingService.SetAsync(new LandingCarouselSettingsDTO());
            }

            await serviceProvider.GetRequiredService<IDeferredDbContext>().SaveChangesAsync();
        }
    }
}
