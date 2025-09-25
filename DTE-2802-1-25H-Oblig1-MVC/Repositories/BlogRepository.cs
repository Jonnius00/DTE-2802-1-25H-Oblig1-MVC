using DTE_2802_1_25H_Oblig1_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DTE_2802_1_25H_Oblig1_MVC.Repositories
{
    /// <summary>
    /// Entity Framework ( EF ) implementation of the IBlogRepository interface.
    /// Provides concrete data access operations for Blog entities using EF Core.
    /// 
    /// Architecture Role:
    /// - Implements Repository Design pattern as required by project specifications
    /// - Encapsulates EF complexity from business logic
    /// - Provides testable abstraction over database operations
    /// - Supports dependency injection for loose coupling
    /// 
    /// Entity Framework Features:
    /// - Uses BlogContext for database access and relationship management
    /// - Leverages EF navigation properties for related data loading
    /// - Implements async patterns for better performance and scalability
    /// - Handles connection management and transaction scope automatically
    /// 
    /// Relationship Handling:
    /// - GetByIdAsync includes Posts for comprehensive blog display
    /// - Supports lazy loading for optional relationship access
    /// - Cascade delete configured through EF relationship configuration
    /// 
    /// Usage in Application:
    /// - Injected into BlogController via dependency injection
    /// - Called from controller actions for all blog data operations
    /// - Enables unit testing with mock repository implementations
    /// - Provides consistent error handling and data access patterns
    /// </summary>
    public class BlogRepository : IBlogRepository
    {
        /// <summary>
        /// EF database context for blog operations.
        /// Provides access to Blogs DbSet and related entities (Posts, Comments).
        /// Injected via dependency injection from Program.cs configuration.
        /// </summary>
        private readonly BlogContext _context;

        /// <summary>
        /// Constructor for dependency injection.
        /// Receives configured BlogContext instance from DI container.
        /// 
        /// Context Configuration:
        /// - Database provider (SQLite, SQL Server, InMemory for testing)
        /// - Connection string and database location
        /// - Identity integration for user management
        /// - Migration and seeding configuration
        /// </summary>
        /// <param name="context">Configured EF BlogContext instance</param>
        public BlogRepository(BlogContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all blogs from the database.
        /// Simple query that returns all Blog entities without related data.
        /// 
        /// Implementation Details:
        /// - Uses ToListAsync() for asynchronous execution
        /// - Does not include Posts to avoid over-fetching for list views
        /// - Returns materialized list (not deferred query)
        /// - Suitable for blog index/listing pages
        /// 
        /// Performance Notes:
        /// - Efficient for moderate numbers of blogs
        /// - Consider pagination for large datasets
        /// - Posts are loaded separately when needed (lazy loading)
        /// </summary>
        /// <returns> Complete list of all Blog entities </returns>
        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific blog by ID with related Posts included.
        /// Uses eager loading to fetch blog and its posts in a single database query.
        /// 
        /// Implementation Strategy:
        /// - Include(b => b.Posts) loads related posts via navigation property
        /// - FirstOrDefaultAsync returns null for non-existent IDs
        /// - Single query execution for better performance than lazy loading
        /// 
        /// Use Cases:
        /// - Blog details page showing blog info and post list
        /// - Post creation form validation (check if blog exists)
        /// - Blog open/close status verification
        /// 
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to retrieve</param>
        /// <returns> Blog with Posts included, or null if not found </returns>
        public async Task<Blog?> GetByIdAsync(int id)
        {
            // return await _context.Blogs.FindAsync(id); // not includeв related posts
            return await _context.Blogs
                              .Include(b => b.Posts)
                              .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Creates a new blog in the database.
        /// Adds the blog entity and saves changes in a single transaction.
        /// 
        /// Implementation Flow:
        /// 1. AddAsync() stages the entity for creation
        /// 2. SaveChangesAsync() commits the transaction to database
        /// 3. EF auto-generates the Id property
        /// 4. Navigation properties are available after save
        /// 
        /// Business Logic Integration:
        /// - OwnerId should be set by controller before calling this method
        /// - IsOpen typically defaults to true for new blogs
        /// - Title and Description come from user input via controller
        /// 
        /// Transaction Management:
        /// - SaveChanges creates atomic transaction
        /// - Rollback occurs automatically if save fails
        /// - Entity state tracking handles change detection
        /// </summary>
        /// <param name="blog">Blog entity to create with populated properties</param>
        public async Task AddAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing blog in the database.
        /// Uses EF change tracking to detect and save modifications.
        /// 
        /// Implementation Details:
        /// - Update() marks all properties as modified
        ///   EF detects which properties actually changed
        ///   EF tracks changes and generates appropriate SQL
        ///   Only modified columns are included in UPDATE statement
        /// 
        /// - SaveChangesAsync() commits changes atomically
        ///   Optimistic concurrency can be configured if needed
        /// 
        /// Usage Note:
        /// - Blog updates not required by specification ( CR only )
        /// - Provided for completeness and future improvements
        /// - Could be used for blog open/close toggle or admin edits
        /// 
        /// </summary>
        /// 
        /// <param name="blog">Blog entity with updated properties and existing Id</param>
        public async Task UpdateAsync(Blog blog)
            {
                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync();
            }

        /// <summary>
        /// Deletes a blog from the database by ID.
        /// Implements safe deletion pattern with existence check.
        /// 
        /// Implementation Flow:
        /// 1. FindAsync() retrieves blog to verify existence
        /// 2. Remove() marks entity for deletion if found
        /// 3. SaveChangesAsync() commits deletion transaction
        /// 4. Cascade delete removes related Posts and Comments
        /// 
        /// Safety Features:
        /// - Null check prevents exceptions for non-existent blogs
        /// - Silent failure for missing entities (idempotent operation)
        /// - Transaction rollback if cascade delete fails
        /// 
        /// </summary>
        /// 
        /// <param name="id">Primary key of the blog to delete</param>
        public async Task DeleteAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
            }
        }
    }
}