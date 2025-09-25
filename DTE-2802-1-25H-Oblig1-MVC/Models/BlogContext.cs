using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DTE_2802_1_25H_Oblig1_MVC.Models
{
  /// <summary>
  /// 
  /// Class creates the Database context for App using EF Core.
  /// Inherits from IdentityDbContext to provide ASP.NET Core Identity integration.
  /// 
  /// Architecture Design:
  /// - Provides centralized DB access for all app entities ( Blog, Post, Comment )
  /// - Combines these entities with Identity user management in single DB
  /// - Supports SQLite DB ( but configurable )
  /// - Implements Repository Design pattern ( data source layer )
  /// 
  /// Entity Framework Features:
  /// - Automatic relationship configuration through navigation properties
  /// - Automatic foreign key constraint generation ( see model classes )
  /// - Cascade delete behavior for parent-child relationships ( as required )
  /// - Migration support for DB schema evolution ( already used under development )
  /// 
  /// Identity Framework Integration:
  /// - Authentication and authorization tables: 
  ///   AspNetUsers: accounts and profile information
  ///   AspNetRoles: roles for authorization ( RBAC not used )
  ///   AspNetUserClaims: Additional  claims and permissions ( not used )
  ///   AspNetUserLogins: External login provider information  ( 3rd party providers not used )
  ///   AspNetUserTokens: Security tokens for password reset, etc. ( not tested yet )
  /// 
  /// - Identity-related DbSets (Users, Roles, ...) are inherited from IdentityDbContext
  ///   and provide  authentication and authorization functionality.
  ///   
  /// - Password management and security features ( comes with IdentityDbContext )
  ///   •Password hashing •Password complexity •Account lockout •2FA ( not implemented )
  ///   
  /// - User registration and login functionality: 
  ///   Creating, login/LOGout, Remember Me, Password reset ( not tested )
  /// 
  /// Usage in Application:
  /// - Injected into controllers and repositories via DI
  /// - Used directly in controllers ( Post,Comment ) for complex queries (along repositories)
  /// - Enables testing with in-memory DB providers ( see RepositoryTests )
  /// 
  /// </summary>
  public class BlogContext : IdentityDbContext
  {

    /// <summary>
    /// Constructor for DI configuration.
    /// Accepts DbContextOptions to configure DB provider and connection string.
    /// 
    /// Configuration Examples:
    /// - SQLite: options.UseSqlite(connectionString)
    /// - In-Memory: options.UseInMemoryDatabase(databaseName) - for testing
    /// - SQL Server: options.UseSqlServer(connectionString) - for production
    /// 
    /// The options parameter is configured in Program.cs in region Service Configuration.
    /// 
    /// </summary>
    /// 
    /// <param name="options">DB configuration options including provider and connection details</param>
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

    /// <summary>
    /// Represents the Blogs table in the DB.
    /// Provides CRUD operations and LINQ query support for Blog entities.
    /// 
    /// Usage Examples:
    /// - context.Blogs.ToListAsync() - Get all blogs
    /// - context.Blogs.Include(b => b.Posts) - Eager load posts with blogs
    /// - context.Blogs.Where(b => b.IsOpen) - Filter open blogs
    /// - context.Blogs.FindAsync(id) - Get blog by primary key
    /// 
    /// Repository Pattern Reflection:
    /// - BlogRepository uses this DbSet for data access operations
    /// - Provides abstraction layer between business logic and data access
    /// - Enables unit testing with mock repositories
    /// 
    /// Entity Configuration:
    /// - Primary key: Blog.Id (auto-generated)
    /// - Relationships: One-to-many with Posts
    /// 
    /// </summary>
    public DbSet<Blog> Blogs { get; set; }
        
    /// <summary>
    /// Represents the Posts table in the DB.
    /// Provides CRUD operations and LINQ query support for Post entities.
    /// 
    /// Usage Examples:
    /// - context.Posts.Include(p => p.Blog) - Load post with parent blog
    /// - context.Posts.Include(p => p.Comments) - Load post with comments
    /// - context.Posts.Where(p => p.BlogId == blogId) - Get posts for specific blog
    /// - context.Posts.OrderByDescending(p => p.Id) - Order posts by creation
    /// 
    /// Relationship Configuration:
    /// - Foreign key: Post.BlogId references Blog.Id
    /// - Navigation properties: Post.Blog (parent), Post.Comments (children)
    /// - Cascade delete: Deleting blog removes all posts
    /// 
    /// Full CRUD Support:
    /// - CREATE: Add new posts to existing blogs
    /// - READ: Display posts in listings and detail views
    /// - UPDATE: Edit post title and content (owner only)
    /// - DELETE: Remove posts and cascade to comments (owner only)
    /// 
    /// </summary>
    public DbSet<Post> Posts { get; set; }
        
    /// <summary>
    /// Represents the Comments table in the DB.
    /// Provides CRUD operations and LINQ query support for Comment entities.
    /// 
    /// Usage Examples:
    /// - context.Comments.Include(c => c.Post) - Load comment with parent post
    /// - context.Comments.Where(c => c.PostId == postId) - Get comments for specific post
    /// - context.Comments.OrderBy(c => c.Id) - Order comments chronologically
    /// - context.Comments.Include(c => c.Post.Blog) - Load comment with post and blog
    /// 
    /// Relationship Configuration:
    /// - Foreign key: Comment.PostId references Post.Id
    /// - Navigation property: Comment.Post (parent)
    /// - Cascade delete: Deleting post removes all comments
    /// 
    /// </summary>
    public DbSet<Comment> Comments { get; set; }
  }
}