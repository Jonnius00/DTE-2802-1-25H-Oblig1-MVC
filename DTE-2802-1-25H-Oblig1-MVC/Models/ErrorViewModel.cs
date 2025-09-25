namespace DTE_2802_1_25H_Oblig1_MVC.Models
{
    /// <summary>
    /// View model for error pages in the application.
    /// Provides error tracking and debugging information 
    /// for both development and production environments.
    /// 
    /// Purpose:
    /// - Displays user-friendly error pages instead of raw exception details
    /// - Provides request tracking for debugging and monitoring
    /// - Maintains application security by not exposing sensitive error details
    /// - Supports troubleshooting by providing correlation identifiers
    /// 
    /// Usage Scenarios:
    /// - Unhandled exceptions are routed to Error controller action
    /// - 404 Not Found errors for missing resources
    /// - 500 Internal Server errors for application failures
    /// - Custom error pages for better user experience
    /// 
    /// Integration with HomeController:
    /// - HomeController.Error() action creates and populates this model
    /// - Error view renders user-friendly error information
    /// - Request tracking helps developers correlate logs with user reports
    /// 
    /// Security Considerations:
    /// - Never exposes sensitive application details to end users
    /// - Production environment shows generic error messages
    /// - Development environment can show detailed error information
    /// - Request ID enables log correlation without exposing internal data
    /// </summary>
    public class ErrorViewModel
    {
      /// <summary>
      /// Unique identifier for tracking the request that caused the error.
      /// Used for correlating user-reported errors with server logs and diagnostics.
      /// 
      /// Value Sources (in priority order):
      /// 1. Activity.Current?.Id - Distributed tracing identifier when available
      /// 2. HttpContext.TraceIdentifier - ASP.NET Core request identifier as fallback
      /// 
      /// Usage:
      /// - Displayed on error pages for user reference when reporting issues
      /// - Used by developers to search logs and find specific error occurrences
      /// - Enables correlation between user experience and server-side diagnostics
      /// - Helps in distributed systems to trace requests across services
      /// 
      /// Example Values:
      /// - Activity ID: "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01"
      /// - Trace ID: "0HM4PG5F91FK2:00000001" (shorter ASP.NET Core format)
      /// 
      /// Privacy Note:
      /// - Safe to display to users as it contains no sensitive information
      /// - Randomly generated identifier that cannot be used to infer user data
      /// </summary>
      public string? RequestId { get; set; }

      /// <summary>
      /// Computed property that determines whether to display the RequestId on the error page.
      /// Provides clean UI logic by hiding empty or null request identifiers.
      /// 
      /// Business Logic:
      /// - Returns true if RequestId has a meaningful value to display
      /// - Returns false if RequestId is null, empty, or whitespace
      /// - Used in Razor views to conditionally render the request ID section
      /// 
      /// View Usage Example:
      /// @if (Model.ShowRequestId)
      /// {
      ///     <p>Request ID: @Model.RequestId</p>
      /// }
      /// 
      /// User Experience:
      /// - Prevents displaying "Request ID: " with no actual ID value
      /// - Keeps error pages clean and professional
      /// - Only shows tracking information when it's actually useful
      /// 
      /// Implementation Note:
      /// - Uses string.IsNullOrEmpty() for robust null/empty checking
      /// - Computed property avoids storing redundant boolean state
      /// - Automatically updates when RequestId changes
      /// </summary>
      public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
