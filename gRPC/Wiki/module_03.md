# Module 3: Database Integration

Integrating Entity Framework Core with gRPC is very similar to REST APIs, but we inject the `DbContext` into the **Service** class instead of a Controller.

## 🎯 Overview

In this module, we'll:
- Set up Entity Framework Core with SQLite
- Create entity models
- Configure the DbContext
- Use dependency injection in gRPC services
- Map entities to Protobuf messages

## 1. Install Required Packages

Add EF Core packages to your project:

```powershell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

Your `.csproj` should include:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
</ItemGroup>
```

## 2. Create Entity Models

Create a `Data/Models` folder and define your entities:

**Data/Models/Post.cs:**

```csharp
namespace gRPC.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? PostImage { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
```

### Key Points:
- **Entity** = Database table representation
- **Protobuf Message** = External API contract
- **Never expose entities directly** - always map to Protobuf messages

## 3. Create the DbContext

**Data/AppDbContext.cs:**

```csharp
using Microsoft.EntityFrameworkCore;
using gRPC.Data.Models;

namespace gRPC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity properties
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedOn).IsRequired();
            });
        }
    }
}
```

## 4. Configure Connection String

**appsettings.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gRPC.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 5. Register DbContext with Dependency Injection

**Program.cs:**

```csharp
using gRPC.Services;
using gRPC.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc();

// Add DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Map gRPC services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<PostService>();

app.Run();
```

## 6. Create and Run Migrations

Create the initial migration:

```powershell
dotnet ef migrations add InitialCreate
```

Apply the migration to create the database:

```powershell
dotnet ef database update
```

This creates `gRPC.db` in your project root.

### Migration Commands Reference:

```powershell
# Add a new migration
dotnet ef migrations add MigrationName

# Update database to latest migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list
```

## 7. Using DbContext in gRPC Services

Inject `AppDbContext` into your service class:

**Services/PostService.cs:**

```csharp
using Grpc.Core;
using gRPC.Data;
using gRPC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPC.Services
{
    public class PostService : gRPC.PostService.PostServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostService> _logger;

        // Constructor injection
        public PostService(AppDbContext context, ILogger<PostService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Create operation
        public override async Task<PostModel> CreatePost(
            CreatePostRequest request,
            ServerCallContext context)
        {
            // Create entity from request
            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                CreatedOn = DateTime.UtcNow
            };

            // Save to database
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created post with ID: {PostId}", post.Id);

            // Map entity to Protobuf message
            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedOn = post.CreatedOn.ToString("o") // ISO 8601 format
            };
        }

        // Read operation
        public override async Task<PostModel> ReadPost(
            ReadPostRequest request,
            ServerCallContext context)
        {
            var post = await _context.Posts.FindAsync(request.Id);

            if (post == null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    $"Post with ID={request.Id} not found."));
            }

            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedOn = post.CreatedOn.ToString("o")
            };
        }

        // List operation
        public override async Task<ListPostsReply> ListPosts(
            ListPostsRequest request,
            ServerCallContext context)
        {
            var posts = await _context.Posts
                .Select(p => new PostModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedOn = p.CreatedOn.ToString("o")
                })
                .ToListAsync();

            var reply = new ListPostsReply();
            reply.Posts.AddRange(posts);

            return reply;
        }
    }
}
```

## 8. Key Differences from REST APIs

| Aspect | REST API (Controller) | gRPC (Service) |
|--------|----------------------|----------------|
| **Injection** | Constructor injection | Same - constructor injection |
| **Return Type** | `IActionResult`, `ActionResult<T>` | Protobuf message types |
| **Error Handling** | HTTP status codes | `RpcException` with `StatusCode` |
| **Serialization** | JSON (automatic) | Protobuf (manual mapping) |
| **Validation** | Model binding | Manual or interceptors |

## 9. Error Handling with RpcException

Use `RpcException` for gRPC-specific errors:

```csharp
// Not Found
throw new RpcException(new Status(StatusCode.NotFound, "Resource not found"));

// Invalid Argument
throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));

// Internal Error
throw new RpcException(new Status(StatusCode.Internal, "Database error"));

// Unauthenticated
throw new RpcException(new Status(StatusCode.Unauthenticated, "Not authenticated"));
```

### Common Status Codes:

- `OK` - Success
- `NotFound` - Resource not found
- `InvalidArgument` - Bad request data
- `AlreadyExists` - Duplicate resource
- `PermissionDenied` - Authorization failed
- `Internal` - Server error
- `Unavailable` - Service unavailable

## 10. Best Practices

### ✅ Do:
- Always map entities to Protobuf messages
- Use async/await for database operations
- Log important operations
- Use proper error handling with `RpcException`
- Validate input before database operations

### ❌ Don't:
- Don't return EF entities directly (they can't be serialized to Protobuf)
- Don't forget to call `SaveChangesAsync()`
- Don't use synchronous database methods
- Don't expose internal error details to clients

## 🎓 Summary

In this module, you learned:
- ✅ How to install and configure Entity Framework Core
- ✅ Creating entity models and DbContext
- ✅ Setting up connection strings and dependency injection
- ✅ Running migrations to create the database
- ✅ Injecting DbContext into gRPC services
- ✅ Mapping entities to Protobuf messages
- ✅ Error handling with RpcException

## 📚 Next Steps

Continue to [Module 4: Professional Patterns](module_04.md) to learn about DTOs, AutoMapper, and service layers.
