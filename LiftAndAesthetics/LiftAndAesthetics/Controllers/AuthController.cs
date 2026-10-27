using Microsoft.AspNetCore.Mvc;
using LiftAndAesthetics.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var result = await _authService.LoginAsync(email, password, false);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Welcome back to our fitness !";
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    TempData["ErrorMessage"] = "Bro, you failed 6 times! Account locked for 15 minutes. Go do some pushups!";
                    return RedirectToAction("Index", new { returnUrl });
                }

                TempData["ErrorMessage"] = "Invalid email or password. Try again bro!";
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill in all fields.";
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var result = await _authService.RegisterAsync(email, password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Account created successfully! Let's get to work.";
                    return LocalRedirect(returnUrl);
                }

                TempData["ErrorMessage"] = string.Join(" | ", result.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill in all fields.";
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _authService.LogoutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}