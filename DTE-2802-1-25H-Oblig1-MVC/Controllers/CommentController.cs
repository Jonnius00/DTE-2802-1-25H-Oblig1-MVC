using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTE_2802_1_25H_Oblig1_MVC.Controllers
{
    public class CommentController : Controller
    {
    private readonly ICommentRepository _commentRepository;
    private readonly BlogContext _context;

        public CommentController(ICommentRepository commentRepository, BlogContext context)
        {
            _commentRepository = commentRepository;
            _context = context;
        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            var comments = await _commentRepository.GetAllAsync();
            return View(comments);
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // GET: Comment/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Content,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var post = await _context.Posts.FindAsync(comment.PostId);
                if (post == null)
                {
                    ModelState.AddModelError(string.Empty, "Post not found.");
                    return View(comment);
                }
                var blog = await _context.Blogs.FindAsync(post.BlogId);
                if (blog == null || !blog.IsOpen)
                {
                    ModelState.AddModelError(string.Empty, "Cannot add comment to a closed blog.");
                    return View(comment);
                }
                // Set owner to current user
                comment.OwnerId = User?.Identity?.Name;
                await _commentRepository.AddAsync(comment);
                // Redirect to the post details page after adding a comment
                return RedirectToAction("Details", "Post", new { id = comment.PostId });
            }
            return View(comment);
        }

        // GET: Comment/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            // Only owner can edit
            if (comment.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            return View(comment);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,PostId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }
            var existingComment = await _commentRepository.GetByIdAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }
            // Only owner can edit
            if (existingComment.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                // Preserve OwnerId
                comment.OwnerId = existingComment.OwnerId;
                await _commentRepository.UpdateAsync(comment);
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comment/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            // Only owner can delete
            if (comment.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            // return View(comment); // VIEW NOT IMPLEMENTED
            await _commentRepository.DeleteAsync(id);
            return RedirectToAction("Details", "Post", new { id = comment.PostId });
    }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            // Only owner can delete
            if (comment.OwnerId != User?.Identity?.Name)
            {
                return Forbid();
            }
            await _commentRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}