using System.Diagnostics;
using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DTE_2802_1_25H_Oblig1_MVC.Controllers
{
    /// <summary>
    /// Main application controller serving as the entry point and providing core navigation.
    /// Handles the default route, error pages, and application-level functionality.
    /// 
    /// Application Architecture Role:
    /// - Serves as the application's front door and navigation hub
    /// - Provides error handling and user-friendly error pages
    /// - Redirects users to main application functionality (blog listing)
    /// - Maintains application-wide pages like Privacy policy
    /// 
    /// Dependencies:
    /// - ILogger for application monitoring and debugging
    /// - IBlogRepository for potential future home page blog integration
    /// - Standard MVC patterns for view rendering and navigation
    /// 
    /// Navigation Strategy:
    /// - Index redirects to Blog listing as primary application function
    /// - Error action provides centralized error handling with request tracking
    /// - Privacy action serves legal compliance content
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger instance for application monitoring and debugging.
        /// Used to record application events, errors, and performance metrics.
        /// Injected via dependency injection from ASP.NET Core logging framework.
        /// </summary>
        private readonly ILogger<HomeController> _logger;
        
        /// <summary>
        /// Repository for blog operations. Currently used for DI setup.
        /// could support potential home page integration
        /// </summary>
        private readonly IBlogRepository _blogRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// Receives logger and blog repository from DI container configuration.
        /// </summary>
        /// <param name="logger">Logger instance for application monitoring</param>
        /// <param name="blogRepository">Repository for blog data access operations</param>
        public HomeController(ILogger<HomeController> logger, IBlogRepository blogRepository)
        {
            _logger = logger;
            _blogRepository = blogRepository;
        }

        /// <summary>
        /// Application landing page that redirects users to the main blog listing.
        /// Implements the primary navigation flow to satisfy requirement #2 (blog overview).
        /// 
        /// HTTP Method: GET
        /// Route: / (root) or /Home/Index
        /// 
        /// Business Logic:
        /// - Immediately redirects to BlogController.Index
        /// - Could display featured blogs on a custom home page
        /// - Could show application statistics or recent activity
        /// 
        /// </summary>
        /// <returns> RedirectToActionResult directing to Blog Index action </returns>
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Blog");
        }

        /// <summary>
        /// Displays the application's privacy policy page.
        /// Provides legal compliance content required for web applications.
        /// 
        /// HTTP Method: GET
        /// Route: /Home/Privacy
        /// 
        /// Legal Compliance:
        /// - Required for GDPR and privacy regulation compliance
        /// - Informs users about data collection and usage
        /// - Provides transparency about application privacy practices
        /// 
        /// Content:
        /// - Static content typically managed through Views/Home/Privacy.cshtml
        /// - May include data processing information
        /// - Contact information for privacy-related inquiries
        /// 
        /// Navigation:
        /// - Typically linked from application footer
        /// - Accessible from all pages for user convenience
        /// - Standard web application requirement
        /// </summary>
        /// <returns> ViewResult displaying the privacy policy content </returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Centralized error handling action for application exceptions and HTTP errors.
        /// Provides user-friendly error pages with request tracking for debugging.
        /// 
        /// HTTP Method: GET
        /// Route: /Home/Error
        /// Caching: [ResponseCache] prevents error page caching
        /// 
        /// Error Handling Strategy:
        /// - Displays generic error message to users (security best practice)
        /// - Provides request tracking ID for debugging correlation
        /// - Maintains application stability during failures
        /// - Logs errors for developer investigation
        /// 
        /// Request Tracking:
        /// - Uses Activity.Current?.Id for distributed tracing when available
        /// - Falls back to HttpContext.TraceIdentifier for request correlation
        /// - Enables log correlation between user reports and server logs
        /// 
        /// Security Considerations:
        /// - Never exposes sensitive application details to users
        /// - Production environment shows generic error messages
        /// - Request ID is safe to display (contains no sensitive data)
        /// 
        /// User Experience:
        /// - Professional error page instead of raw exception details
        /// - Clear indication that an error occurred
        /// - Request ID for user to reference when reporting issues
        /// 
        /// Development vs Production:
        /// - Development: May show detailed error information (configured separately)
        /// - Production: Shows user-friendly generic error page
        /// - Request tracking works in both environments
        /// </summary>
        /// <returns> ViewResult with ErrorViewModel containing request tracking information </returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
