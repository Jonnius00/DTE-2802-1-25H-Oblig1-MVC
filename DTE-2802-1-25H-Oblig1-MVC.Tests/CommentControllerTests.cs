using DTE_2802_1_25H_Oblig1_MVC.Controllers;
using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DTE_2802_1_25H_Oblig1_MVC.Tests
{
    public class CommentControllerTests
    {
        /// <summary>
        /// Tests the Index action method of CommentController.
        /// Verifies that the controller correctly retrieves all comments from the repository
        /// and returns them as a ViewResult with the appropriate model type.
        /// 
        /// Requirements covered:
        /// - Comment listing functionality (requirement #3: all comments should be displayed for each post)
        /// - Repository pattern usage with ICommentRepository
        /// - MVC ViewResult return type with proper model binding
        /// 
        /// Test scenario: GET /Comment/Index
        /// Expected: ViewResult with IEnumerable&lt;Comment&gt; model containing all comments
        /// 
        /// Implementation details:
        /// - Mocks ICommentRepository.GetAllAsync() to return empty list
        /// - Uses in-memory database for BlogContext dependency
        /// - This action is typically used for administrative purposes or debugging
        /// </summary>
        [Fact]
        public async Task Index_ReturnsViewWithComments()
        {
            var mockRepo = new Mock<ICommentRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Comment>());

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "CommentTestDatabase1")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new CommentController(mockRepo.Object, blogContext);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Comment>>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a comment exists.
        /// Verifies that the controller correctly retrieves a specific comment by ID
        /// and returns it as a ViewResult with the Comment model.
        /// 
        /// Requirements covered:
        /// - Comment read functionality (requirement #3: comments display)
        /// - Individual comment access with ID-based routing
        /// - Repository pattern with GetByIdAsync method
        /// 
        /// Test scenario: GET /Comment/Details/1 (where comment with ID=1 exists)
        /// Expected: ViewResult with Comment model containing the requested comment
        /// 
        /// Implementation details:
        /// - Mocks ICommentRepository.GetByIdAsync(1) to return a comment with ID=1
        /// - Uses separate in-memory database instance to avoid test interference
        /// - This action allows viewing individual comment details
        /// </summary>
        [Fact]
        public async Task Details_CommentExists_ReturnsViewWithComment()
        {
            var mockRepo = new Mock<ICommentRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Comment { Id = 1, Content = "Test Comment" });

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "CommentTestDatabase2")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new CommentController(mockRepo.Object, blogContext);

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Comment>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a comment does not exist.
        /// Verifies that the controller properly handles missing comment scenarios
        /// by returning a NotFoundResult instead of throwing exceptions.
        /// 
        /// Requirements covered:
        /// - Error handling for non-existent resources
        /// - Proper HTTP status code responses (404 Not Found)
        /// - Repository pattern with null return handling
        /// 
        /// Test scenario: GET /Comment/Details/1 (where comment with ID=1 does not exist)
        /// Expected: NotFoundResult (HTTP 404)
        /// 
        /// Implementation details:
        /// - Mocks ICommentRepository.GetByIdAsync(1) to return null
        /// - Uses separate in-memory database instance for isolation
        /// - Follows RESTful API conventions for resource not found scenarios
        /// </summary>
        [Fact]
        public async Task Details_CommentNotFound_ReturnsNotFound()
        {
            var mockRepo = new Mock<ICommentRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Comment)null);

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "CommentTestDatabase3")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new CommentController(mockRepo.Object, blogContext);

            var result = await controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Tests the Create GET action method of CommentController.
        /// Verifies that the controller returns a view for comment creation form.
        /// This action requires authentication via [Authorize] attribute.
        /// 
        /// Requirements covered:
        /// - Comment creation functionality (requirement #1: only logged-in users can create comments)
        /// - Form display for user input
        /// - GET request handling for create operations
        /// - Authentication requirement enforcement
        /// 
        /// Test scenario: GET /Comment/Create (authenticated user)
        /// Expected: ViewResult that displays the comment creation form
        /// 
        /// Note: Authentication testing would require more complex setup with mocked User context.
        /// This test focuses on the basic functionality assuming authentication passes.
        /// </summary>
        [Fact]
        public void Create_Get_ReturnsView()
        {
            var mockRepo = new Mock<ICommentRepository>();
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "CommentTestDatabase4")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new CommentController(mockRepo.Object, blogContext);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }
    }
}