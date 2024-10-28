using Microsoft.AspNetCore.Mvc;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;
using UCR.WEB.Blog.ViewModels;

namespace UCR.WEB.Blog.Controllers;

public class LoginController : Controller
{
    private readonly BlogDbContext _context;
    public LoginController(BlogDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserVM model)
    {
        if (model.Password != model.ConfirmationKey)
        {
            ViewData["Error"] = "Passwords do not match";
            return View();
        }

        User user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = model.Password,
            Role = "Author"
        };

        if (ModelState.IsValid)
        {
           await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        if(user.Id != 0)return RedirectToAction("Index", "Home");
        ViewData["Error"] = "User could not be created";

        return View();
    }
}
