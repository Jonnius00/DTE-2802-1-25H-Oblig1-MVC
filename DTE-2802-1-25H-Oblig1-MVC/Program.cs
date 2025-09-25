using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Application startup and configuration file for the Educational Blog System.
/// Configures all services, middleware, and application behavior using ASP.NET Core minimal hosting model.
/// 
/// Architecture Overview:
/// - Implements Repository Design pattern for all entity types.
///   Repository services created for data access abstraction.
/// - Uses Entity Framework Core with SQLite database.
/// - Integrates ASP.NET Core Identity for user management and authentication.
/// - Follows MVC architectural pattern with Razor views.
///   Controller and View configuration are made for web interface.
/// - Implements dependency injection for loose coupling.
/// 
/// Service Configuration (region below): 
/// - Registers MVC services with Views support.
/// - Configures Entity Framework DbContext with DB provider
/// - Configures ASP.NET Core Identity
/// - Registers Repository Services
/// 
/// Request Pipeline (region below):
/// - Exception handling for production environments
/// - HTTPS redirection for security
/// - Static file serving for CSS/JS/images
/// - Authentication and authorization middleware
/// - MVC routing for controller actions
/// - Razor Pages for Identity UI
/// </summary>

var builder = WebApplication.CreateBuilder(args);

#region Service Configuration

/// <summary>
/// Registers MVC services with Views support.
/// Enables controller actions, model binding, view rendering, and Razor syntax.
/// Required for the MVC architectural pattern implementation.
/// </summary>
builder.Services.AddControllersWithViews();

/// <summary>
/// Configures Entity Framework DbContext with SQLite database provider.
/// 
/// Connection String:
/// - Retrieved from appsettings.json configuration
/// - Typically points to local SQLite database file
/// - Supports both development and production scenarios
/// 
/// </summary>
builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BlogContext")));

/// <summary>
/// Configures ASP.NET Core Identity for user authentication and management.
/// Implements requirement #1: "Users must log in to create a blog, post new posts and comments"
/// 
/// Identity Features:
/// - User registration and login functionality
/// - Password hashing and security
/// - User session management
/// - Role-based authorization (if needed in future)
/// - Default UI pages for authentication flows
/// 
/// Integration:
/// - Uses BlogContext for Identity tables storage
/// - Shares database with blog entities
/// - Provides User.Identity.Name for OwnerId tracking
/// - Enables [Authorize] attribute functionality
/// 
/// </summary>
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<BlogContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

/// <summary>
/// Registers Repository pattern implementations for DI.
/// Implements the Repository Design pattern 
/// 
/// Service Lifetime: Scoped
/// - New instance per HTTP request
/// - Ensures proper DbContext lifecycle management
/// - Supports transaction scope per request
/// 
/// Registered Services:
/// - IBlogRepository → BlogRepository: Blog CR operations
/// - IPostRepository → PostRepository: Post CRUD operations  
/// - ICommentRepository → CommentRepository: Comment CRUD operations
/// 
/// </summary>
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

#endregion

var app = builder.Build();

#region Request Pipeline Configuration

/// <summary>
/// Configures exception handling for production environments.
/// Routes unhandled exceptions to HomeController.Error action for user-friendly error pages.
/// 
/// Development vs Production:
/// - Development: Shows detailed exception pages for debugging
/// - Production: Shows generic error page to protect sensitive information
/// 
/// Error Handling Flow:
/// - Exception occurs → Middleware catches → Redirects to /Home/Error
/// - HomeController.Error creates ErrorViewModel with request tracking
/// - User sees professional error page instead of raw exception details
/// 
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    /// <summary>
    /// Enables HTTP Strict Transport Security (HSTS) for production.
    /// Forces browsers to use HTTPS connections for enhanced security.
    /// Default: 30 days (recommended to adjust for production scenarios).
    /// </summary>
    app.UseHsts();
}

/// <summary>
/// Redirects HTTP requests to HTTPS for security.
/// Ensures all communication is encrypted, protecting user data and authentication tokens.
/// Critical for login functionality and sensitive blog content.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Enables serving static files (CSS, JavaScript, images) from wwwroot folder.
/// Required for:
/// - Bootstrap CSS framework for responsive UI
/// - jQuery for client-side interactivity
/// - jQuery Validation for client-side form validation
/// - Custom CSS and JavaScript files
/// - Application images and icons
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Enables routing for URL-to-controller-action mapping.
/// Required before authentication/authorization middleware to determine endpoints.
/// Supports conventional routing and attribute routing patterns.
/// </summary>
app.UseRouting();

/// <summary>
/// Enables authentication middleware for user login/logout functionality.
/// Must be placed after UseRouting() and before UseAuthorization().
/// 
/// Functionality:
/// - Processes authentication cookies and tokens
/// - Populates User.Identity with current user information
/// - Enables User.Identity.Name for OwnerId tracking
/// - Supports [Authorize] attribute requirements
/// </summary>
app.UseAuthentication();

/// <summary>
/// Enables authorization middleware for access control.
/// Must be placed after UseAuthentication() and before endpoint mapping.
/// 
/// Features:
/// - Processes [Authorize] and [AllowAnonymous] attributes
/// - Enforces ownership-based authorization (requirement #5)
/// - Supports role-based authorization if implemented
/// - Returns appropriate HTTP status codes (401, 403)
/// </summary>
app.UseAuthorization();

/// <summary>
/// Configures default MVC routing pattern.
/// Maps URLs to controller actions using conventional routing.
/// 
/// Pattern: {controller=Home}/{action=Index}/{id?}
/// Examples:
/// - / → HomeController.Index → Redirects to BlogController.Index
/// - /Blog → BlogController.Index → Shows all blogs
/// - /Blog/Details/5 → BlogController.Details(5) → Shows specific blog
/// - /Post/Create → PostController.Create → Shows post creation form
/// 
/// Default Values:
/// - controller=Home: Default to HomeController when no controller specified
/// - action=Index: Default to Index action when no action specified
/// - id?: Optional parameter for entity identifiers
/// 
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Maps Razor Pages for ASP.NET Core Identity UI.
/// Provides pre-built pages for user registration, login, password management, etc.
/// 
/// Identity Pages:
/// - /Identity/Account/Register: User registration
/// - /Identity/Account/Login: User login
/// - /Identity/Account/Logout: User logout
/// - /Identity/Account/Manage/*: User profile management
/// 
/// Integration:
/// - Works alongside MVC controllers
/// - Shares authentication state with controller actions
/// - Provides consistent user experience
/// </summary>
app.MapRazorPages();

/// <summary>
/// Custom middleware to REDIRECT USERS TO BLOG LISTING after successful login.
/// - User logs in → Default redirect → Custom middleware → /Blog listing
/// 
/// Functionality:
/// - Intercepts POST requests to Identity login page
/// - Modifies redirect location from default to /Blog
/// - Ensures users see blog content immediately after authentication
/// 
/// Implementation Note:
/// - Uses Response.OnStarting to modify headers before response is sent
/// - Checks for 302 redirect status and modifies Location header
/// - Preserves other authentication functionality
/// </summary>
app.Use(async (context, next) =>
{
  if (context.Request.Path == "/Identity/Account/Login" && context.Request.Method == "POST")
  {
    context.Response.OnStarting(() =>
    {
      if (context.Response.StatusCode == 302 && context.Response.Headers["Location"].ToString().Contains("/"))
      {
          context.Response.Headers["Location"] = "/Blog";
      }
      return Task.CompletedTask;
    });
    }
    await next();
});

#endregion

/// <summary>
/// Starts the web application and begins listening for HTTP requests.
/// entry point that launches the entire system.
/// 
/// Application Lifecycle:
/// 1. Service configuration (dependency injection setup)
/// 2. Middleware pipeline configuration (request processing)
/// 3. Application startup (app.Run())
/// 4. Request processing begins
/// 5. Application remains running until stopped
/// 
/// Ready State:
/// - Database context configured and ready
/// - Identity system operational for user authentication
/// - Repository pattern available for data access
/// - MVC controllers ready to handle requests
/// - Static files and UI components available
/// - Error handling and security measures active
/// 
/// </summary>
app.Run();
