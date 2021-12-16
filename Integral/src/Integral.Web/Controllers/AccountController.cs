using Integral.Application.Common.Security;
using Integral.Infrastructure.Identity;
using Integral.Web.Models.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Integral.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<ApplicationUser> signInManager, 
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _signInManager = signInManager;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginRedirect));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _signInManager
                .PasswordSignInAsync(model.UserName, model.Password, 
                    isPersistent: false, lockoutOnFailure: !_environment.IsDevelopment());

            if (!result.Succeeded)
            {
                _logger.LogWarning("Invalid login attempt.");

                ModelState.AddModelError(string.Empty, "Произошла ошибка");
                return View();
            }

            return RedirectToAction(nameof(LoginRedirect), new { model.ReturnUrl });
        }

        [HttpGet]
        public IActionResult LoginRedirect(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            if (User.IsInRole(Role.Admin))
            {
                return RedirectToAction(nameof(AdminController.Index), "Admin");
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        //private IActionResult PersonalSettings()
        //{
        //    return View();
        //}
    }
}