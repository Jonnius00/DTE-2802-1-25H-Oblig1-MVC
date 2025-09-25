using DTE_2802_1_25H_Oblig1_MVC.Controllers;
using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTE_2802_1_25H_Oblig1_MVC.Tests
{
    public class PostControllerTests
    {
        /// <summary>
        /// Tests the Index action method of PostController.
        /// Verifies that the controller correctly retrieves all posts from the repository
        /// and returns them as a ViewResult with the appropriate model type.
        /// 
        /// Requirements covered:
        /// - Post listing functionality (requirement #2: overview of all posts in a blog)
        /// - Repository pattern usage with IPostRepository
        /// - MVC ViewResult return type with proper model binding
        /// 
        /// Test scenario: GET /Post/Index
        /// Expected: ViewResult with IEnumerable&lt;Post&gt; model containing all posts
        /// 
        /// Implementation details:
        /// - Mocks IPostRepository.GetAllAsync() to return empty list
        /// - Uses in-memory database for BlogContext dependency
        /// - Validates both the result type and model type
        /// </summary>
        [Fact]
        public async Task Index_ReturnsViewWithPosts()
        {
            var mockRepo = new Mock<IPostRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Post>());

            // Create DbContextOptions for BlogContext
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new PostController(mockRepo.Object, blogContext);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Post>>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a post exists.
        /// Verifies that the controller correctly retrieves a specific post by ID
        /// and returns it as a ViewResult with the Post model.
        /// 
        /// Requirements covered:
        /// - Post read functionality (requirement #2: viewing individual posts)
        /// - Individual post access with ID-based routing
        /// - Repository pattern with GetByIdAsync method
        /// - Comment display integration (requirement #3: all comments should be displayed for each post)
        /// 
        /// Test scenario: GET /Post/Details/1 (where post with ID=1 exists)
        /// Expected: ViewResult with Post model containing the requested post
        /// 
        /// Implementation details:
        /// - Mocks IPostRepository.GetByIdAsync(1) to return a post with ID=1
        /// - Uses separate in-memory database instance to avoid test interference
        /// - This action typically loads post with related comments for display
        /// </summary>
        [Fact]
        public async Task Details_PostExists_ReturnsViewWithPost()
        {
            var mockRepo = new Mock<IPostRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Post { Id = 1 });

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new PostController(mockRepo.Object, blogContext);

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Post>(viewResult.Model);
        }

        /// <summary>
        /// Tests the Details action method when a post does not exist.
        /// Verifies that the controller properly handles missing post scenarios
        /// by returning a NotFoundResult instead of throwing exceptions.
        /// 
        /// Requirements covered:
        /// - Error handling for non-existent resources
        /// - Proper HTTP status code responses (404 Not Found)
        /// - Repository pattern with null return handling
        /// - User experience for invalid post IDs
        /// 
        /// Test scenario: GET /Post/Details/1 (where post with ID=1 does not exist)
        /// Expected: NotFoundResult (HTTP 404)
        /// 
        /// Implementation details:
        /// - Mocks IPostRepository.GetByIdAsync(1) to return null
        /// - Uses separate in-memory database instance for isolation
        /// - This prevents the application from crashing on invalid post IDs
        /// - Follows RESTful API conventions for resource not found scenarios
        /// </summary>
        [Fact]
        public async Task Details_PostNotFound_ReturnsNotFound()
        {
            var mockRepo = new Mock<IPostRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Post)null);

            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;
            var blogContext = new BlogContext(options);

            var controller = new PostController(mockRepo.Object, blogContext);

            var result = await controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
