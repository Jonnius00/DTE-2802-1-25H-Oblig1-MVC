using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTE_2802_1_25H_Oblig1_MVC.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly BlogContext _context;
        public PostController(IPostRepository postRepository, BlogContext context)
        {
            _postRepository = postRepository;
            _context = context;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            var posts = await _postRepository.GetAllAsync();
            return View(posts);
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // GET: Post/Create
        [Authorize]
        public IActionResult Create(int? blogId)
        {
            var post = new Post();
            if (blogId.HasValue)
            {
                post.BlogId = blogId.Value;
            }
            return View(post);
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // validates hidden XSRF token given by antiforgery token generator in View file
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,BlogId")] Post post)
        {
            if (ModelState.IsValid)
            {
                var blog = await _context.Blogs.FindAsync(post.BlogId);
                if (blog == null || !blog.IsOpen)
                {
                    ModelState.AddModelError(string.Empty, "Cannot add post to a closed blog.");
                    return View(post);
                }
                // Set owner to current user
                post.OwnerId = User?.Identity?.Name;
                await _postRepository.AddAsync(post);
                // return RedirectToAction(nameof(Index)); // separate view for all posts
                return RedirectToAction("Details", "Blog", new { id = post.BlogId });

      }
      return View(post);
        }

        // GET: Post/Edit/5
        // fetches the movie from DB by ID and populates the edit form, returns HTTP 404 otherwise
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            // Only owner can edit
            if (post.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            return View(post);
        }

        // POST: Post/Edit/5
        // MISSING: Ownership check here! - FIXED
        // Any authenticated user can edit any post by directly posting to this endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,BlogId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound(); // Oops, ID mismatch, possibly tampering
            }

            // Get the existing post to check ownership
            var existingPost = await _postRepository.GetByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound(); // has already been deleted
            }
            // Only owner can edit
            if ( existingPost.OwnerId != User?.Identity?.Name )
            {
                return Forbid();
            }

            if (ModelState.IsValid) // Validation checks OK
            {   
                post.OwnerId = existingPost.OwnerId; // Preserve the original OwnerId

                // await _postRepository.UpdateAsync(post); // Simpler but no concurrency handling
                try
                {
                  await _postRepository.UpdateAsync(post);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( await _postRepository.GetByIdAsync(post.Id) == null )
                        return NotFound();  // has already been deleted
                    else
                        throw;
                }
                // return RedirectToAction(nameof(Index)); // separate view for all posts
                return RedirectToAction("Details", "Blog", new { id = post.BlogId });

            }
            return View(post); // If movie object has any validation errors
                               // Edit method redisplays View form
        }

        // GET: Post/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            // Only owner can delete
            if (post.OwnerId != User?.Identity?.Name)
            {
              return Forbid();
            }
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            // Only owner can delete
            if (post.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            await _postRepository.DeleteAsync(id);
            // return RedirectToAction(nameof(Index)); // separate view for all posts
            return RedirectToAction("Details", "Blog", new { id = post.BlogId });
        }
    }
}