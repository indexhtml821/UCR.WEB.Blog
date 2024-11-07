using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UCR.WEB.Blog.Models;
using Microsoft.EntityFrameworkCore;
using UCR.WEB.Blog.Models.Data;

namespace UCR.WEB.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, BlogDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts.ToList();
            var users = _context.Users.ToList();
            var viewModel = new PostUserModel
            {
                Posts = posts,
                Users = users
            };

            ViewData["HeaderText"] = "Diario Web";
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            ViewData["HeaderText"] = "Privacidad";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
