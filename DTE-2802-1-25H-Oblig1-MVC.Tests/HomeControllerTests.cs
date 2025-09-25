using DTE_2802_1_25H_Oblig1_MVC.Controllers;
using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DTE_2802_1_25H_Oblig1_MVC.Tests
{
    public class HomeControllerTests
    {
        /// <summary>
        /// Tests the Index action method of HomeController.
        /// Verifies that the application's main entry point correctly redirects users
        /// to the blog listing page, which shows an overview of all available blogs.
        /// 
        /// Requirements covered:
        /// - Application navigation flow
        /// - Requirement #2: "It should be possible to get an overview of which blogs exist"
        /// - User experience: landing page leads to main functionality
        /// 
        /// Test scenario: GET / (root URL) or GET /Home/Index
        /// Expected: RedirectToActionResult that redirects to "Index" action on "Blog" controller
        /// 
        /// Implementation details:
        /// - Mocks required dependencies (ILogger and IBlogRepository)
        /// - Validates that the redirect target is correct
        /// - This design ensures users immediately see available content upon arrival
        /// </summary>
        [Fact]
        public void Index_RedirectsToBlogIndex()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockBlogRepo = new Mock<IBlogRepository>();
            var controller = new HomeController(mockLogger.Object, mockBlogRepo.Object);

            // Act
            var result = controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Blog", redirectResult.ControllerName);
        }

        /// <summary>
        /// Tests the Privacy action method of HomeController.
        /// Verifies that the controller correctly serves the privacy policy page
        /// as a standard ViewResult, which is required for web application compliance.
        /// 
        /// Requirements covered:
        /// - Legal compliance (privacy policy accessibility)
        /// - Static content serving
        /// - Standard web application navigation
        /// 
        /// Test scenario: GET /Home/Privacy
        /// Expected: ViewResult that displays the privacy policy page
        /// 
        /// Implementation details:
        /// - Tests basic ViewResult return without complex model binding
        /// - This is typically a static content page with legal information
        /// - Required for GDPR and other privacy regulation compliance
        /// </summary>
        [Fact]
        public void Privacy_ReturnsView()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockBlogRepo = new Mock<IBlogRepository>();
            var controller = new HomeController(mockLogger.Object, mockBlogRepo.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Tests the Error action method of HomeController.
        /// Verifies that the controller correctly handles error scenarios by returning
        /// an appropriate error view with tracking information for debugging purposes.
        /// 
        /// Requirements covered:
        /// - Error handling and user experience
        /// - Request tracking for debugging and monitoring
        /// - Graceful failure handling instead of application crashes
        /// 
        /// Test scenario: GET /Home/Error (typically reached via error routing)
        /// Expected: ViewResult with ErrorViewModel containing request tracking information
        /// 
        /// Implementation details:
        /// - Mocks HttpContext to provide TraceIdentifier for tracking
        /// - Validates that ErrorViewModel is properly populated
        /// - Error action has [ResponseCache] attribute to prevent caching of error pages
        /// - Uses Activity.Current?.Id or HttpContext.TraceIdentifier for request tracking
        /// - This helps with debugging and monitoring application issues
        /// </summary>
        [Fact]
        public void Error_ReturnsViewWithErrorModel()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockBlogRepo = new Mock<IBlogRepository>();
            var controller = new HomeController(mockLogger.Object, mockBlogRepo.Object);
            
            // Mock HttpContext for TraceIdentifier
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("test-trace-id");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            // RequestId should be either Activity.Current?.Id or HttpContext.TraceIdentifier
            Assert.True(!string.IsNullOrEmpty(model.RequestId));
        }

        /// <summary>
        /// Tests the Error action method when Activity.Current is available.
        /// Verifies that the controller prioritizes Activity.Current.Id over HttpContext.TraceIdentifier
        /// for more detailed request tracking when available.
        /// 
        /// Test scenario: GET /Home/Error with active Activity context
        /// Expected: ViewResult with ErrorViewModel using Activity.Current.Id for RequestId
        /// 
        /// Note: This test validates the priority of tracking identifiers:
        /// 1. Activity.Current?.Id (when available)
        /// 2. HttpContext.TraceIdentifier (fallback)
        /// </summary>
        [Fact]
        public void Error_WithActivity_UsesActivityId()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockBlogRepo = new Mock<IBlogRepository>();
            var controller = new HomeController(mockLogger.Object, mockBlogRepo.Object);
            
            // Mock HttpContext
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(x => x.TraceIdentifier).Returns("trace-id");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Create an Activity to simulate Activity.Current
            using var activity = new Activity("TestActivity");
            activity.Start();

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            
            // Should use Activity.Current.Id when available
            Assert.Equal(activity.Id, model.RequestId);
        }
    }
}