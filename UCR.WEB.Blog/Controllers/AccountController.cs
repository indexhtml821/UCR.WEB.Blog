using Microsoft.AspNetCore.Mvc;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace UCR.WEB.Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["HeaderText"] = "Registro";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserVM model)
        {
            ViewData["HeaderText"] = "Registro";

            if (model.Password != model.ConfirmationKey)
            {
                ViewData["Error"] = "Passwords do not match";
                return View();
            }

            var user = new User
            {
                UserName = model.Email, // Identity requires UserName to be set
                Email = model.Email,
                Name = model.Name,
            //    Role = "Author", // Optional, may need to handle roles differently in ASP.NET Identity
                Password = model.Password
            };

            if (ModelState.IsValid)
            {
                // Create the user using UserManager
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optionally add the role to the user if using roles
                    await _userManager.AddToRoleAsync(user, "Author");

                    return RedirectToAction("Index", "Home");
                }

                // Add errors if user creation failed
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Error"] = "User could not be created";
            return View();
        }

    [HttpGet]
    public IActionResult Login()
    {
        ViewData["HeaderText"] = "Iniciar Sesión";
        return View();
    }


        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            ViewData["HeaderText"] = "Iniciar Sesión";
            // Verify if the user exists
            var foundUser = await _userManager.FindByEmailAsync(model.Email);
            if (foundUser == null)
            {
                ViewData["Error"] = "User not found";
                return View();
            }

            // Log in using SignInManager
            var result = await _signInManager.PasswordSignInAsync(foundUser, model.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Error"] = "Invalid login attempt";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
