using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTE_2802_1_25H_Oblig1_MVC.Controllers
{
  /// <summary>
  /// MVC Controller for Blog entity operations.
  /// Implements CR (Create, Read) operations for blogs.
  /// 
  /// Requirements Implementation:
  /// - Requirement #2: Blog overview - Index action shows all blogs
  /// - Requirement #4: Blog CREATE and READ operations
  /// - Requirement #1: Authentication required for blog creation
  /// - Requirement #6: Blog open/close feature for content control
  /// - Requirement #7: Parameter passing via GET/POST (no sessions)
  /// 
  /// Authorization Strategy:
  /// - [Authorize] at CLASS LEVEL requires authentication for most actions
  /// - [AllowAnonymous] on Index/Details allows public blog viewing
  /// - Blog creation restricted to authenticated users only
  /// - Ownership tracking via User.Identity.Name
  /// 
  /// Repository Pattern:
  /// - Uses IBlogRepository for data access abstraction
  /// - Dependency injection provides loose coupling
  /// - Enables unit testing with mock repositories
  /// - Separates business logic from data persistence
  /// 
  /// MVC Architecture:
  /// - Actions return ViewResults for form display
  /// - POST actions handle form submissions with model binding
  /// - RedirectToAction prevents duplicate submissions
  /// - Model validation ensures data integrity
  /// </summary>
  /// 
  /// <remarks>
  /// Constructor, receives repository implementation from DI container
  /// </remarks>
  /// <param name="blogRepository">Repository implementation for blog data access</param>
  [Authorize]
    public class BlogController(IBlogRepository blogRepository) : Controller
    {
        /// <summary>
        /// Repository for blog data access operations.
        /// Injected via DI to support Repository Design pattern.
        /// Provides abstraction over Entity Framework operations.
        /// </summary>
        private readonly IBlogRepository _blogRepository = blogRepository;

        /// <summary>
        /// Displays a listing of all blogs in the system.
        /// This is the main entry point for blog browsing
        /// 
        /// [AllowAnonymous] - Public access for blog discovery
        /// HTTP Method: GET
        /// Route: /Blog or /Blog/Index
        /// 
        /// Business Logic:
        /// - Retrieves all blogs via repository pattern
        /// - No filtering applied - shows all blogs regardless of open/closed status
        /// 
        /// View Data:
        /// - Passes IEnumerable<Blog> to view for rendering
        /// - View shows Title, Description, Owner, OpenStatus, Action links
        /// - May include post count and blog status indicators ( NOT IMPLEMENTED )
        /// 
        /// Navigation Flow:
        /// - HomeController.Index redirects here as main application entry
        /// - Blog creation and other operations redirect back here
        /// - Serves as application dashboard for blog discovery
        /// </summary>
        /// <returns>  ViewResult with collection of all Blog entities </returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogRepository.GetAllAsync();
            return View(blogs);
        }

        /// <summary>
        /// Displays detailed view of a specific blog with its posts.
        /// Shows blog information and lists all posts within the blog.
        /// 
        /// [AllowAnonymous] - Public access for content viewing
        /// HTTP Method: GET
        /// Route: /Blog/Details/{id}
        /// 
        /// Business Logic:
        /// - Retrieves blog by ID including related Posts
        /// - Returns 404 if blog doesn't exist
        /// - Shows blog open/closed status for content creation context
        /// 
        /// View Data:
        /// - Passes Blog entity with Posts navigation property populated
        /// - View displays blog title, description, status, and post listing
        /// - Provides links to create new posts (if blog is open and user authenticated)
        /// 
        /// Error Handling:
        /// - NotFound() for invalid blog IDs provides proper HTTP 404 response
        /// - Graceful handling prevents application crashes on bad URLs
        /// </summary>
        /// <param name="id">Primary key of the blog to display</param>
        /// <returns>  ViewResult with Blog entity or NotFoundResult if blog doesn't exist </returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound(); // StatusCodes.Status404NotFound response
      }
            return View(blog);
        }

        /// <summary>
        /// Displays the blog creation form.
        /// First step in the blog creation process 4: CREATE operation).
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: GET
        /// Route: /Blog/Create
        /// 
        /// Business Logic:
        /// - Only authenticated users can access blog creation
        /// - Returns EMPTY form for user input
        /// - Form includes Title, Description, IsOpen fields
        /// 
        /// Form Flow:
        /// - GET: Display empty form
        /// - POST: Process form submission (Create POST action)
        /// - Success: Redirect to Index
        /// - Validation Error: Redisplay form with errors
        /// </summary>
        /// <returns>  ViewResult with empty Blog modelg </returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes blog creation form submission.
        /// Implements the CREATE operation for blogs 4).
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: POST
        /// Route: /Blog/Create
        /// [ValidateAntiForgeryToken] - CSRF protection
        /// 
        /// Business Logic:
        /// - Validates model data (Title, Description required)
        /// - Sets IsOpen = true for new blogs (allows content creation)
        /// - Sets OwnerId to current authenticated user
        /// - Creates blog via repository pattern
        /// 
        /// Model Binding:
        /// - [Bind] attribute prevents over-posting attacks
        /// - Excludes sensitive properties like OwnerId from form binding
        /// - Manual property assignment ensures data integrity
        /// 
        /// Success Flow:
        /// - Valid model: Create blog → Redirect to Index
        /// - Shows user their new blog in the listing
        /// 
        /// Error Flow:
        /// - Invalid model: Return form with validation errors for input correction
        /// </summary>
        /// 
        /// <param name="blog">Blog model from form submission</param>
        /// <returns> RedirectToActionResult on success, ViewResult with errors on validation failure </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsOpen")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.IsOpen = true;
                // Set owner to current user
                blog.OwnerId = User?.Identity?.Name;
                await _blogRepository.AddAsync(blog);
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        /// <summary>
        /// Displays the blog editing form.
        /// 
        /// NOTE: Blog editing is NOT required by the specification (only CR operations).
        /// provided for completeness and potential future requirements.
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: GET
        /// Route: /Blog/Edit/{id}
        /// 
        /// Security Considerations ( currently MISSING ):
        /// - include ownership verification 
        /// - Only blog owner access edit functionality
        /// - adding authorization policies
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to edit</param>
        /// <returns>  ViewResult with Blog model or NotFoundResult </returns>
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        /// <summary>
        /// Processes blog editing form submission.
        /// 
        /// NOTE: Blog editing is NOT required by the specification (only CR operations).
        /// provided for completeness and potential future requirements.
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: POST
        /// Route: /Blog/Edit/{id}
        /// [ValidateAntiForgeryToken] - CSRF protection
        /// 
        /// Security Issues ( TODO ):
        /// - Missing ownership verification
        /// - check if User.Identity.Name == blog.OwnerId
        /// - preserve OwnerId during update
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to update</param>
        /// <param name="blog">Updated blog model from form</param>
        /// <returns>  RedirectToActionResult on success, ViewResult with errors on failure </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsOpen")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _blogRepository.UpdateAsync(blog);
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        /// <summary>
        /// Displays blog deletion confirmation page.
        /// 
        /// NOTE: Blog deletion is NOT required by the specification (only CR operations).
        /// provided for completeness and cascade delete testing.
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: GET
        /// Route: /Blog/Delete/{id}
        /// 
        /// Cascade Delete Impact:
        /// - Deleting blog will remove all associated posts
        /// - Deleting posts will remove all associated comments
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to delete</param>
        /// <returns>  ViewResult with Blog model for confirmation or NotFoundResult </returns>
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        /// <summary>
        /// Processes blog deletion confirmation.
        /// 
        /// NOTE: Blog deletion is NOT required by the specification (only CR operations).
        /// provided for completeness and cascade delete testing.
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: POST
        /// Route: /Blog/Delete/{id}
        /// [ValidateAntiForgeryToken] - CSRF protection
        /// 
        /// Cascade Behavior:
        /// - Repository implementation handles cascade delete through EF
        /// - All posts and comments are automatically removed
        /// - Maintains referential integrity
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to delete</param>
        /// <returns>  RedirectToActionResult to Index after deletion </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _blogRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Opens a blog to allow new posts and comments.
        /// Implements the blog open/close feature.
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: GET
        /// Route: /Blog/Open/{id}
        /// 
        /// Business Logic:
        /// - Sets blog.IsOpen = true
        /// - Enables post andmcomment creation for this blog
        /// - Updates blog via repository pattern
        /// 
        /// User Experience:
        /// - Redirects back to blog Details page
        /// - User can immediately see the status change
        /// - Post creation links become available
        /// 
        /// Security Note ( CURRENTLY MISSING ):
        /// - include ownership verification 
        /// - Only blog owner control open/close status
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to open</param>
        /// <returns>  RedirectToActionResult to Details page or NotFoundResult  </returns>
        public async Task<IActionResult> Open(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            blog.IsOpen = true;
            await _blogRepository.UpdateAsync(blog);
            return RedirectToAction(nameof(Details), new { id = blog.Id });
        }

        /// <summary>
        /// Closes a blog to prevent new posts and comments.
        /// Implements the blog open/close feature 6).
        /// 
        /// [Authorize] - Requires authentication
        /// HTTP Method: GET
        /// Route: /Blog/Close/{id}
        /// 
        /// Business Logic:
        /// - Sets blog.IsOpen = false
        /// - Prevents new post and comment creation for this blog
        /// - Existing content remains accessible for viewing
        /// 
        /// Impact on Other Controllers:
        /// - PostController.Create checks blog.IsOpen before allowing post creation
        /// - CommentController.Create checks via comment.Post.Blog.IsOpen
        /// - UI hides creation links for closed blogs
        /// 
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to close</param>
        /// <returns>  RedirectToActionResult to Details page or NotFoundResult </returns>
        public async Task<IActionResult> Close(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            blog.IsOpen = false;
            await _blogRepository.UpdateAsync(blog);
            return RedirectToAction(nameof(Details), new { id = blog.Id });
        }
    }
}
