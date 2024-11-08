using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;
using System.Security.Claims;

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
            var posts = _context.Posts.Include(p => p.Comments).ToList();
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
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string commentText)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound();
            }
            DateTime currentDateTime = DateTime.Now;

            var comment = new Comment
            {
                Text = commentText,
                PostId = postId,
                Post = post,
                CreationDateTime = currentDateTime,
                UserName = @User.Identity.Name
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == postId);

            return RedirectToAction("Index");
        }
    }
}
