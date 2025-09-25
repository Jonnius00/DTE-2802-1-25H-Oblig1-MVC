# Main Project Documentation for DTE-2802-1-25H-Oblig1-MVC Solution

## Overview

Introduction & Project Overview (2 minutes) 
- Show: Solution structure in Visual Studio/VS Code.

## Architecture Summary

### 🏗️ Application Architecture consit of following part

- follows ASP.NET Core **MVC** with **Repository** Design **Pattern**
- for **Visual design** were used  ASP.NET Core Razor Pages .
- **Data persistance**: Entity Framework Core with SQLite database
- **Authentication**: implements using ASP.NET Core Identity
- **Security**: secures using HTTPS, CSRF protection, ownership-based authorization

### Authentication & Authorization Foundation (2 minutes) ###
Show:
Login and registration functionality (ASP.NET Core Identity).
Attempt to access create functionality without login (redirect to login).
Only logged-in users can create blogs, posts, and comments.

- **Models**: OwnerId properties in all entities
- **Controllers**: use [Authorize] attributes 
- **Program.cs**: adds Identity service configuration
- **Demonstration**: Show login requirement and ownership tracking

### 📊 Entity Relationships
The solution has a hierarchical structure with three main entity classes: Blog, Post, and Comment. 
This architecture supports ownership and authorization functionality as it requires initially.
These classes are interconnected through Entity Framework relationships. 
Each Blog can contain multiple Posts, and each Post can have multiple Comments.

```
Blog (1:N) → Post (1:N) → Comment
    ↓           ↓           ↓
  OwnerId    OwnerId    OwnerId
    ↓           ↓           ↓
IdentityUser ←───────────────┘
```

## 📁 Documented Classes

### Data Models & Relationships (1.5 minutes). Show Blog.cs,  Post.cs, Comment.cs and model classes.
Entity relationships organized such as Blog contains Posts, Posts contain Comments.
Each has OwnerId property for ownership tracking across all entities.
All have Navigation properties for Entity Framework relationships.

#### 1. Blog.cs ✅
**Purpose**: Top-level container entity for the blogging system. Contains typical properties like Id, Title, Description.

**Key functionality**:
- **IsOpen Property**: Controls whether new posts/comments can be added (requirement #6)
- **OwnerId**: Links to authenticated user for ownership tracking (requirement #1)
- **Navigation Properties**: One-to-many relationship with Posts
- **Business Rules**: Only CR operations required (requirement #4)

**Presentation Points**:
- Show blog listing functionality (requirement #2: blog overview)
- Demonstrate blog creation by authenticated users
- Blog open/close feature controls content creation (requirement #6)
- Show ownership tracking and authorization
- Blog details page displays blog info and its posts

#### 2. Post.cs ✅
**Purpose**: Middle-level entity containing blog content and comments

**Key functionality**:
- **Full CRUD Support**: CREATE, READ, UPDATE, DELETE operations (requirement #4)
- **Ownership Control**: Only owner can edit/delete (requirement #5)
- **Blog Relationship**: Must belong to exactly one blog (BlogId foreign key)
- **Comment Container**: One-to-many relationship with Comments
- **Cascade Delete**: Removing post removes all comments (requirement #5)

**Presentation Points**:
- Demonstrate full CRUD operations with authorization
- Show post listing within blogs
- Post details page displays post content and all its comments (requirement #3)
- Post creation/editing forms for authenticated users
- Explain ownership verification for edit/delete
- Post deletion with ownership verification and comment cascade
- Show cascade delete behavior

#### 3. Comment.cs ✅
**Purpose**: Last-level entity for user discussions and feedback

**Key functionality**:
- **Content Validation**: Built-in validation prevents empty comments
- **Post Relationship**: Must belong to exactly one post (PostId foreign key)
- **Ownership Control**: Only owner can edit/delete (requirement #5)
- **Blog Status Check**: Can only be created in open blogs (requirement #6)

**Presentation Points**:
- Comments are displayed on post details pages below the post content
- Demonstrate comment CRUD with ownership verification
- Explain blog open/close impact on comment creation
- Show content validation in action
- Comment creation forms allow users to add responses to posts
- Comment editing/deletion with ownership verification

#### 4. BlogContext.cs ✅
**Purpose**: Entity Framework database context with Identity integration

**Key functionality**:
- **Identity Integration**: Inherits from IdentityDbContext for user management
- **DbSets**: Blogs, Posts, Comments with relationship configuration
- **SQLite Support**: As specified in requirements (preferably SQLite)
- **Migration Support**: Database schema evolution and seeding

**Presentation Points**:
- Explain Repository pattern data access
- Show database relationship configuration
- Demonstrate migration and database setup
- Explain Identity integration benefits

#### 5. ErrorViewModel.cs ✅
**Purpose**: Error handling and request tracking for debugging

**Key functionality**:
- **Request Tracking**: Activity IDs and trace identifiers for debugging
- **User-Friendly Errors**: Professional error pages instead of raw exceptions
- **Security**: No sensitive information exposed to users
- **Conditional Display**: Only shows request ID when meaningful

**Presentation Points**:
- Show error handling in action
- Explain request tracking for debugging
- Demonstrate graceful failure handling

## Repository Pattern & Architecture
Now, let's examine our architecture using the Repository Design pattern. This pattern separates data access from business logic, providing a clean and maintainable structure.

I'll show you our repository interface and implementation files, demonstrating how dependency injection is used throughout our solution.

#### 6. IBlogRepository.cs ✅
**Purpose**: Repository pattern interface for Blog data access

**Key functionality**:
- **Design Pattern**: Abstracts data access from business logic
- **Async Operations**: All methods async for performance
- **CRUD Interface**: Standard Create, Read, Update, Delete operations
- **Testability**: Enables unit testing with mock implementations

**Presentation Points**:
- Explain Repository Design pattern benefits
- Show dependency injection and interface usage
- Demonstrate separation of concerns
- Explain testing advantages

#### 7. BlogRepository.cs ✅
**Purpose**: Entity Framework implementation of blog repository

**Key functionality**:
- **Entity Framework**: Concrete implementation using EF Core
- **Relationship Loading**: Eager loading with Include() for Posts
- **Transaction Management**: Automatic transaction handling
- **Error Handling**: Safe deletion with existence checks

**Presentation Points**:
- Show Entity Framework implementation details
- Demonstrate relationship loading strategies
- Explain transaction and error handling
- Show cascade delete behavior

## Controller Layer

#### 8. BlogController.cs ✅
**Purpose**: MVC controller for blog-related HTTP requests. Implements CR (Create, Read) operations for blogs.

**Key functionality**.

**Authorization Strategy:**
   - [Authorize] at CLASS LEVEL requires authentication for most actions
   - [AllowAnonymous] on Index/Details allows public blog viewing
   - Blog creation restricted to authenticated users only
   - Ownership tracking via User.Identity.Name
-
- **Repository Pattern**: 
 - Dependency injection provides loose coupling
 - Enables unit testing with mock repositories
 - Uses IBlogRepository for data access abstraction
 - Separates business logic from data persistence
-
- **CR Operations**: CREATE and READ as specified (requirement #4)
- **Open/Close**: Actions to toggle blog status (requirement #6)
- **Ownership**: Sets OwnerId for new blogs (requirement #1)
- **URL Parameter** passing via GET and POST requests (no sessions used).

**Presentation Points**:
- Show blog listing (requirement #2: overview)
- Demonstrate blog creation by authenticated users
- Show blog details with post listing
- Explain open/close feature functionality
- Show authorization and ownership tracking

#### 9. HomeController.cs ✅
**Purpose**: Application entry point and navigation controller

**Key functionality**:
- **Navigation Hub**: Redirects to main blog listing
- **Error Handling**: Centralized error page with request tracking
- **Privacy Compliance**: Legal requirement page
- **Dependency Injection**: Logger and repository integration

**Presentation Points**:
- Show application entry point behavior
- Demonstrate error handling and tracking
- Explain navigation flow to blog overview
- Show professional error pages

#### 10. Program.cs ✅
**Purpose**: 
 Application startup and configuration file for the Educational Blog System.
 Configures all services, middleware, and application behavior using ASP.NET Core minimal hosting model.

**Key functionality**:
- **Service Registration**: 
- Registers MVC services with Views support.
- Configures Entity Framework DbContext with DB provider
- Configures ASP.NET Core Identity
- Registers Repository Services
-
- **Middleware Pipeline**: 
 - Exception handling for production environments
 - HTTPS redirection for security
 - Static file serving for CSS/JS/images
 - Authentication and authorization middleware
 - MVC routing for controller actions
 - Razor Pages for Identity UI
-
- **Custom Routing**: Login redirect to blog listing

**Presentation Points**:
- Explain application architecture setup
- Show dependency injection configuration
- Demonstrate security middleware pipeline
- Explain database and authentication setup
- Show custom user experience enhancements

