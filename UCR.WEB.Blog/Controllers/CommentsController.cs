using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;

namespace UCR.WEB.Blog.Controllers;

public class CommentsController:Controller
{
    private readonly BlogDbContext _context;

    public CommentsController(BlogDbContext context)
    {
        _context = context;
    }

    public string Index()
    {
        return "This is my default action...";
    }

    // 
    // GET: /HelloWorld/Welcome/ 
    public string Welcome()
    {
        return "This is the Welcome action method...";
    }


    // GET: Movies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }

        return View(comment);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([Bind("Id,Text,PostId")] Comment comment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { id = comment.PostId });
        }
        return View(comment);
    }
}
