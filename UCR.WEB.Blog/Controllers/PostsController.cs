using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UCR.WEB.Blog.Models;
using UCR.WEB.Blog.Models.Data;

namespace UCR.WEB.Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;

        public PostsController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            ViewData["HeaderText"] = "Publicaciones";

            // Cargar los posts y sus comentarios asociados
            var postsWithComments = await _context.Posts
                .Include(p => p.Comments)  // Incluir los comentarios asociados
                .ToListAsync();

            return View(postsWithComments);
        }

        [Authorize]
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["HeaderText"] = "Detalles";

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Comments)  // Cargar los comentarios del post
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["HeaderText"] = "Crear un nuevo post";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Post post, IFormFile ImageData)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["HeaderText"] = "Crear Publicación";
            post.UserId = userId;
            _context.Add(post);
            await _context.SaveChangesAsync();
            if (ModelState.IsValid)
            {
                if (ImageData != null && ImageData.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImageData.CopyToAsync(memoryStream);
                        post.ImageData = memoryStream.ToArray();
                    }
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["HeaderText"] = "Editar el Post";
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"userId: {userId}, post.UserId: {post.UserId}, IsInRole(Admin): {User.IsInRole("Administrator")}");


            if (userId != post.UserId && !User.IsInRole("Administrator"))
            {
                return Forbid(); // El usuario no est� autorizado a editar este post
            }

            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,UserId")] Post post)
        {
            ViewData["HeaderText"] = "Editar el Post";

            if (id != post.Id)
            {
                return NotFound();
            }

            // Verificar que el usuario actual es el autor o un administrador antes de proceder
            var existingPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId != existingPost.UserId && !User.IsInRole("Administrator"))
            {
                return Forbid(); // El usuario no est� autorizado a editar este post
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Asegurarse de no modificar el UserId
                    post.UserId = existingPost.UserId;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["HeaderText"] = "Eliminar el Post";

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            // Obtener el UserId del usuario actual
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Verificar si el usuario es el autor o un administrador
            if (userId != post.UserId && !User.IsInRole("Administrator"))
            {
                return Forbid(); // El usuario no est� autorizado a ver esta p�gina
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["HeaderText"] = "Eliminar el Post";

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                // Obtener el UserId del usuario actual
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Verificar si el usuario es el autor o un administrador
                if (userId != post.UserId && !User.IsInRole("Administrator"))
                {
                    return Forbid(); // El usuario no est� autorizado a eliminar este post
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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

            var comment = new Comment
            {
                Text = commentText,
                PostId = postId,
                Post = post
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
