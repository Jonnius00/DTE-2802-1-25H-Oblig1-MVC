using DTE_2802_1_25H_Oblig1_MVC.Controllers;
using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTE_2802_1_25H_Oblig1_MVC.Tests
{
    public class BlogControllerTests
    {
        /// <summary>
        /// Tests the Index action method of BlogController.
        /// Verifies that the controller correctly retrieves all blogs from the repository
        /// and returns them as a ViewResult with the appropriate model type.
        /// 
        /// Requirements covered:
        /// - Blog listing functionality (requirement #2: overview of which blogs exist)
        /// - Repository pattern usage
        /// - MVC ViewResult return type
        /// 
        /// Test scenario: GET /Blog/Index
        /// Expected: ViewResult with IEnumerable<Blog>; model containing all blogs
        /// </summary>
        [Fact]
        // MethodName_Scenario_ExpectedBehaviour()
        public async Task Index_ReturnsViewWithBlogs()
        {
            // AAA convention
            // Arrange | where object initialization happens
            var mockRepo = new Mock<IBlogRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Blog>());
            var controller = new BlogController(mockRepo.Object);

            // Act | where the method under test is called   
            var result = await controller.Index();

            // Assert | where the results are verified 
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Blog>>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a blog exists.
        /// Verifies that the controller correctly retrieves a specific blog by ID
        /// and returns it as a ViewResult with the Blog model.
        /// 
        /// Requirements covered:
        /// - Blog read functionality (requirement #2: blog details viewing)
        /// - Individual blog access
        /// - Repository pattern with GetByIdAsync method
        /// 
        /// Test scenario: GET /Blog/Details/1 (where blog with ID=1 exists)
        /// Expected: ViewResult with Blog model containing the requested blog
        /// </summary>
        [Fact]
        public async Task Details_BlogExists_ReturnsViewWithBlog()
        {
            var mockRepo = new Mock<IBlogRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Blog { Id = 1 });
            var controller = new BlogController(mockRepo.Object);

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Blog>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a blog does not exist.
        /// Verifies that the controller properly handles missing blog scenarios
        /// by returning a NotFoundResult instead of throwing exceptions.
        /// 
        /// Requirements covered:
        /// - Error handling for non-existent resources
        /// - Proper HTTP status code responses (404 Not Found)
        /// - Repository pattern with null return handling
        /// 
        /// Test scenario: GET /Blog/Details/1 (where blog with ID=1 does not exist)
        /// Expected: NotFoundResult (HTTP 404)
        /// </summary>
        [Fact]
        public async Task Details_BlogNotFound_ReturnsNotFound()
        {
            var mockRepo = new Mock<IBlogRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Blog)null);
            var controller = new BlogController(mockRepo.Object);

            var result = await controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Tests the Create GET action method of BlogController.
        /// Verifies that the controller returns a view for blog creation form.
        /// This is the first step in the blog creation process.
        /// 
        /// Requirements covered:
        /// - Blog creation functionality (requirement #4: CREATE operation for blogs)
        /// - Form display for user input
        /// - GET request handling for create operations
        /// 
        /// Test scenario: GET /Blog/Create
        /// Expected: ViewResult that displays the blog creation form
        /// 
        /// Note: This test covers the GET action for Create. The POST action test
        /// would require more complex setup including user authentication and model validation.
        /// </summary>
        [Fact]
        public void Create_Get_ReturnsView()
        {
            var mockRepo = new Mock<IBlogRepository>();
            var controller = new BlogController(mockRepo.Object);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }
    }
}
