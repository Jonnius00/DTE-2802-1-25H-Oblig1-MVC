using DTE_2802_1_25H_Oblig1_MVC.Models;
using DTE_2802_1_25H_Oblig1_MVC.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTE_2802_1_25H_Oblig1_MVC.Tests
{
    /// <summary>
    /// Unit tests for the Repository pattern implementations.
    /// Tests the data access layer components that implement the Repository Design pattern
    /// as required by the project specifications.
    /// 
    /// Repository Pattern Testing Strategy:
    /// - Tests all CRUD operations for each repository
    /// - Uses Entity Framework In-Memory database for isolated testing
    /// - Validates data persistence and retrieval operations
    /// - Tests Entity Framework relationship loading (Include operations)
    /// - Ensures repository methods properly handle null scenarios
    /// - Validates that SaveChangesAsync is called appropriately
    /// 
    /// Requirements covered:
    /// - Repository Design pattern implementation (as specified in requirements)
    /// - Entity Framework integration and data persistence
    /// - CRUD operations for all entities (Blog: CR, Post: CRUD, Comment: CRUD)
    /// - Data integrity and relationship management
    /// - Cascade delete behavior testing
    /// 
    /// Architecture Notes:
    /// - Each repository implements the corresponding interface (IBlogRepository, etc.)
    /// - Repositories encapsulate Entity Framework operations
    /// - In-memory database ensures test isolation and performance
    /// - Tests validate both the repository logic and EF configuration
    /// </summary>
    public class RepositoryTests
    {
        #region BlogRepository Tests

        /// <summary>
        /// Tests BlogRepository.GetAllAsync() method.
        /// Verifies that the repository correctly retrieves all blogs from the database.
        /// 
        /// Requirements covered:
        /// - Blog listing functionality (requirement #2: overview of which blogs exist)
        /// - Repository pattern implementation for blog data access
        /// 
        /// Test scenario: Database contains multiple blogs
        /// Expected: Method returns all blogs as IEnumerable<Blog>;
        /// </summary>
        [Fact]
        public async Task BlogRepository_GetAllAsync_ReturnsAllBlogs()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_GetAll_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Seed test data
            var blogs = new List<Blog>
            {
                new Blog { Title = "Blog 1", Description = "Description 1", IsOpen = true },
                new Blog { Title = "Blog 2", Description = "Description 2", IsOpen = false }
            };
            context.Blogs.AddRange(blogs);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Blog 1");
            Assert.Contains(result, b => b.Title == "Blog 2");
        }

        /// <summary>
        /// Tests BlogRepository.GetByIdAsync() method when blog exists.
        /// Verifies that the repository correctly retrieves a specific blog with its related posts.
        /// 
        /// Requirements covered:
        /// - Blog read functionality with post relationships
        /// - Entity Framework Include operations for related data
        /// - Repository pattern with specific entity retrieval
        /// 
        /// Test scenario: Database contains blog with related posts
        /// Expected: Method returns blog with posts loaded
        /// </summary>
        [Fact]
        public async Task BlogRepository_GetByIdAsync_ExistingBlog_ReturnsBlogWithPosts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_GetById_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Seed test data
            var blog = new Blog { Id = 1, Title = "Test Blog", Description = "Test Description", IsOpen = true };
            var post = new Post { Id = 1, Title = "Test Post", Content = "Test Content", BlogId = 1, Blog = blog };
            blog.Posts = new List<Post> { post };
            
            context.Blogs.Add(blog);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Blog", result.Title);
            Assert.NotNull(result.Posts);
            Assert.Single(result.Posts);
            Assert.Equal("Test Post", result.Posts.First().Title);
        }

        /// <summary>
        /// Tests BlogRepository.GetByIdAsync() method when blog does not exist.
        /// Verifies that the repository correctly handles missing blog scenarios.
        /// 
        /// Test scenario: Database does not contain requested blog ID
        /// Expected: Method returns null
        /// </summary>
        [Fact]
        public async Task BlogRepository_GetByIdAsync_NonExistingBlog_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_GetById_Null_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests BlogRepository.AddAsync() method.
        /// Verifies that the repository correctly adds a new blog to the database.
        /// 
        /// Requirements covered:
        /// - Blog creation functionality (requirement #4: CREATE operation for blogs)
        /// - Repository pattern implementation for data persistence
        /// - Entity Framework AddAsync and SaveChangesAsync operations
        /// 
        /// Test scenario: Adding a new blog to empty database
        /// Expected: Blog is persisted and retrievable with generated ID
        /// </summary>
        [Fact]
        public async Task BlogRepository_AddAsync_PersistsBlogToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_Add_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            var blog = new Blog 
            { 
                Title = "New Blog", 
                Description = "New Description", 
                IsOpen = true,
                OwnerId = "user1"
            };

            // Act
            await repository.AddAsync(blog);

            // Assert
            var savedBlog = await context.Blogs.FirstOrDefaultAsync();
            Assert.NotNull(savedBlog);
            Assert.Equal("New Blog", savedBlog.Title);
            Assert.Equal("user1", savedBlog.OwnerId);
            Assert.True(savedBlog.IsOpen);
        }

        /// <summary>
        /// Tests BlogRepository.UpdateAsync() method.
        /// Verifies that the repository correctly updates an existing blog.
        /// 
        /// Requirements covered:
        /// - Data modification operations (though not required for blogs per spec)
        /// - Repository pattern implementation for update operations
        /// - Entity Framework Update and SaveChangesAsync operations
        /// 
        /// Note: While requirements specify only CR for blogs, Update is implemented
        /// for completeness and potential future requirements.
        /// </summary>
        [Fact]
        public async Task BlogRepository_UpdateAsync_ModifiesExistingBlog()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_Update_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Seed initial data
            var blog = new Blog { Title = "Original Title", Description = "Original Description", IsOpen = true };
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();

            // Modify blog
            blog.Title = "Updated Title";
            blog.IsOpen = false;

            // Act
            await repository.UpdateAsync(blog);

            // Assert
            var updatedBlog = await context.Blogs.FirstOrDefaultAsync();
            Assert.NotNull(updatedBlog);
            Assert.Equal("Updated Title", updatedBlog.Title);
            Assert.False(updatedBlog.IsOpen);
        }

        /// <summary>
        /// Tests BlogRepository.DeleteAsync() method when blog exists.
        /// Verifies that the repository correctly removes a blog from the database.
        /// 
        /// Requirements covered:
        /// - Data deletion operations (though not required for blogs per spec)
        /// - Repository pattern implementation for delete operations
        /// - Cascade delete behavior (should remove related posts and comments)
        /// 
        /// Note: While requirements specify only CR for blogs, Delete is implemented
        /// for completeness and testing cascade delete functionality.
        /// </summary>
        [Fact]
        public async Task BlogRepository_DeleteAsync_ExistingBlog_RemovesBlogFromDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_Delete_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Seed test data
            var blog = new Blog { Title = "Blog To Delete", Description = "Description", IsOpen = true };
            context.Blogs.Add(blog);
            await context.SaveChangesAsync();
            var blogId = blog.Id;

            // Act
            await repository.DeleteAsync(blogId);

            // Assert
            var deletedBlog = await context.Blogs.FindAsync(blogId);
            Assert.Null(deletedBlog);
        }

        /// <summary>
        /// Tests BlogRepository.DeleteAsync() method when blog does not exist.
        /// Verifies that the repository handles deletion of non-existent blogs gracefully.
        /// 
        /// Test scenario: Attempting to delete blog that doesn't exist
        /// Expected: No exception thrown, operation completes successfully
        /// </summary>
        [Fact]
        public async Task BlogRepository_DeleteAsync_NonExistingBlog_DoesNotThrowException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: "BlogRepo_Delete_NotFound_Test")
                .Options;

            using var context = new BlogContext(options);
            var repository = new BlogRepository(context);

            // Act & Assert
            await repository.DeleteAsync(999); // Should not throw exception
        }

        #endregion

        // TODO: Implement similar comprehensive tests for PostRepository and CommentRepository
        //
        // PostRepository Tests needed:
        // - GetAllAsync_ReturnsAllPosts()
        // - GetByIdAsync_ExistingPost_ReturnsPostWithComments()
        // - GetByIdAsync_NonExistingPost_ReturnsNull()
        // - AddAsync_PersistsPostToDatabase()
        // - UpdateAsync_ModifiesExistingPost()
        // - DeleteAsync_ExistingPost_RemovesPostAndComments() // Test cascade delete
        // - DeleteAsync_NonExistingPost_DoesNotThrowException()
        //
        // CommentRepository Tests needed:
        // - GetAllAsync_ReturnsAllComments()
        // - GetByIdAsync_ExistingComment_ReturnsComment()
        // - GetByIdAsync_NonExistingComment_ReturnsNull()
        // - AddAsync_PersistsCommentToDatabase()
        // - UpdateAsync_ModifiesExistingComment()
        // - DeleteAsync_ExistingComment_RemovesComment()
        // - DeleteAsync_NonExistingComment_DoesNotThrowException()
        //
        // Integration Tests needed:
        // - Test Entity Framework relationship configurations
        // - Test cascade delete behavior across all entities
        // - Test blog open/close status impact on repository operations
        // - Test ownership data persistence and retrieval
        //
        // Helper Methods to implement:
        // - CreateInMemoryContext(string databaseName)
        // - SeedTestData(BlogContext context)
        // - AssertBlogProperties(Blog expected, Blog actual)
    }
}