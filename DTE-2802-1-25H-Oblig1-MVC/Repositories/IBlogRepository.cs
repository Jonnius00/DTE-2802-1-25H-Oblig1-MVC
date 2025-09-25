using DTE_2802_1_25H_Oblig1_MVC.Models;

namespace DTE_2802_1_25H_Oblig1_MVC.Repositories
{
  /// <summary>
  /// Repository INTERFACE for Blog entity operations.
  /// Defines the contract for data access on entities 
  /// using the Repository Design pattern.
  /// 
  /// Implementation Notes:
  /// - All methods are async for better performance
  /// - Returns nullable Blog? for methods that might not find entities
  /// - Uses IEnumerable for read operations to support deferred execution: 
  ///   Query is only executed when the results are actually needed, 
  ///   such as when the code iterates over. 
  ///   Can chain multiple operations (filtering, sorting) 
  ///   on the IEnumerable before the query is executed
  /// - Repository implementations handle Entity Framework specifics
  /// 
  /// Requirements Mapping:
  /// - GetAllAsync, GetByIdAsync: blog overview, Read operations
  /// - AddAsync: Create operation for blogs
  /// - UpdateAsync, DeleteAsync: Not required but provided for completeness
  /// 
  /// Usage in Controllers:
  /// - Injected via dependency injection in BlogController
  /// - Controllers call repository methods instead of using DbContext directly
  /// - Enables clean separation between Presentation and Data layers
  /// </summary>
  public interface IBlogRepository
    {
        /// <summary>
        /// Retrieves all blogs from the data store.
        /// Used for blog listing pages and overview functionality.
        /// 
        /// Returns:
        /// - IEnumerable of all Blog entities in the system
        /// - Empty collection if no blogs exist
        /// - Includes related Posts for display purposes
        /// 
        /// Business Usage:
        /// - Blog index page showing all available blogs
        /// - Home page redirection to blog listing
        /// 
        /// </summary>
        Task<IEnumerable<Blog>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific blog by its unique ID.
        /// Includes related Posts for comprehensive blog display.
        /// 
        /// Parameters:
        /// - id: Primary key of the blog to retrieve
        /// 
        /// Returns:
        /// - Blog entity with related Posts if found
        /// - null if no blog exists with the specified ID
        /// 
        /// Business Usage:
        /// - Blog details page showing blog info and its posts
        /// - Verification that blog exists before creating posts
        /// - Authorization checks for blog ownership
        /// - Blog open/close status validation
        /// 
        /// Implementation Notes:
        /// - Includes Posts navigation property to let display all posts in a blog
        /// - Used in controllers for displaying blog details
        /// - Null return enables proper 404 Not Found responses
        /// </summary>
        Task<Blog?> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new blog in the data store.
        /// Implements the CREATE portion of blog CR operations
        /// 
        /// Parameters:
        /// - blog: Blog entity to create with populated properties
        /// 
        /// Business Rules:
        /// - OwnerId should be set to current authenticated user
        /// - IsOpen defaults to TRUE for new blogs
        /// - Title and Description should be provided by user input
        /// 
        /// Implementation Notes:
        /// - Entity Framework will auto-generate the Id property
        /// - SaveChanges should be called to persist to database
        /// - validation at repository or service layer NOT IMPLEMENTED
        /// </summary>
        Task AddAsync(Blog blog);

        /// <summary>
        /// Updates an existing blog in the data store.
        /// 
        /// NOTE: Blog updates are NOT required by the specification (only CR operations).
        /// method is provided for completeness and potential future impprovements.
        /// 
        /// Parameters:
        /// - blog: Blog entity with updated properties and existing Id
        /// 
        /// Potential Usage:
        /// - Blog title/description editing 
        /// - Blog open/close status toggling
        /// - Ownership transfer ( admin functions)
        /// 
        /// Security Considerations:
        /// - ownership verification in calling controller - NOT IMPLEMENTED
        /// - requiring authorization attributes and policies - NOT IMPLEMENTED
        /// - OwnerId should not be modified through this method
        /// </summary>
        Task UpdateAsync(Blog blog);

        /// <summary>
        /// Deletes a blog from the data store.
        /// 
        /// NOTE: Blog deletion is NOT required by the specification (only CR operations).
        /// method is provided for completeness and cascade delete testing.
        /// 
        /// Parameters:
        /// - id: Primary key of the blog to delete
        /// 
        /// Cascade Behavior:
        /// - Implements "container object deletion removes all children".
        ///   Deleting blog should remove all associated posts.
        ///   Deleting posts should remove all associated comments.
        /// 
        /// Security Considerations  - NOT IMPLEMENTED:
        /// - Should require elevated permissions (admin or owner)
        /// - Consider soft delete vs hard delete for data retention
        /// - May need confirmation UI due to destructive nature
        /// </summary>
        Task DeleteAsync(int id);
    }
}