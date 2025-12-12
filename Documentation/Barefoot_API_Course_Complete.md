# 🦶 Barefoot API: Mastering Minimal APIs in .NET 8

Welcome to the **Barefoot API** course! This guide will take you step-by-step from zero to a professional, production-ready Minimal API using clean architecture principles.

## 📚 Course Modules

| Module | Title | Description |
| :--- | :--- | :--- |
| **[Module 1: Fundamentals](/module_01.md)** | **Project Setup & Basics** | Environment setup, project creation, directory structure, and your first "Hello World". |
| **[Module 2: The First Steps](/module_02.md)** | **Static Data & Basic Endpoints** | Working with in-memory data, parameter binding, and customizing Swagger documentation. |
| **[Module 3: Database Integration](/module_03.md)** | **EF Core & Clean Architecture** | Connecting to SQLite/SQL Server, defining Models, Context, and running Migrations. |
| **[Module 4: Professional Patterns](/module_04.md)** | **Services, DTOs & Mapping** | Decoupling logic with Services, using DTOs to hide internal models, and AutoMapper. |
| **[Module 5: Data Mastery](/module_05.md)** | **Search, Sort & Pagination** | Handling large datasets efficiently with filtering, sorting, and pagination. |
| **[Module 6: Robustness](/module_06.md)** | **Validation, Error Handling & Files** | Validating inputs with FluentValidation, global error handling, and file uploads. |
| **[Module 7: Security](/module_07.md)** | **Authentication & Authorization** | Securing your API with JWT Bearer tokens and configuring CORS. |

---

## 🛠️ Tech Stack
- **.NET 8**
- **Entity Framework Core**
- **Sqlite / SQL Server**
- **AutoMapper**
- **FluentValidation**
- **Swagger / OpenAPI**

> [!NOTE]
> This course is based on the **Barefoot API** project architecture.
# Module 1: Fundamentals & Setup

In this module, we will establish the foundation of our Minimal API project. We'll start from scratch, understand the project structure, and get our first endpoint running.

## 1. Create the Project
We will use the .NET CLI to create a new Minimal API project.

```powershell
# Create a new Web API project
dotnet new webapi -minimal -n MinAPI

# Navigate into the project directory
cd MinAPI
```

> [!TIP]
> The `-minimal` flag ensures we get the lightweight Minimal API template without Controllers.

**Note on JSON**: This project includes `Microsoft.AspNetCore.Mvc.NewtonsoftJson`. While .NET 8 uses `System.Text.Json` by default, Newtonsoft is often added for specific serialization behaviors or legacy compatibility.

## 2. The `.gitignore` File
Git is essential. A proper `.gitignore` prevents clutter (like `bin/`, `obj/`, `user secrets`) from hitting your repository.

**Create a `.gitignore`** at the root of your solution with standard .NET content:

```gitignore
## Ignore Visual Studio temporary files, build results, and user-specific files.
bin/
obj/
.vs/
.vscode/
*.user
appsettings.local.json
```

## 3. Project Structure
Your new project looks like this. Let's create the folder structure we will need effectively implementing **Clean Architecture** later.

```text
MinAPI/
├── Program.cs          # Entry point & Configuration
├── MinAPI.csproj       # Project file & Dependencies
├── appsettings.json    # Configuration (Connections, Logging)
│
├── Data/               # Database Context & Repositories
├── Data/Models/        # Entity Models
├── Endpoints/          # API Route Groups (Endpoints)
├── Extensions/         # Extension methods for clean Program.cs
├── Services/           # Business Logic
├── DTOs/               # Data Transfer Objects
└── Validations/        # Input Validation Rules
```

> [!IMPORTANT]
> Creating these folders now helps us stay organized as the project grows.

## 4. Understanding `Program.cs`
In Minimal APIs, `Program.cs` is where everything happens.
It has three main zones:

1.  **Builder & Services Zone**: Registration of DI (Dependency Injection) containers.
2.  **Build**: The app is built.
3.  **Middleware & Pipelines**: Request handling pipeline (Services, Swaggers, Https).

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES ZONE
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. MIDDLEWARE ZONE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. ENDPOINTS ZONE
app.MapGet("/weatherforecast", () => { ... })
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
```

## Next Step
Now that we have the project created, let's start adding **Static Data** and building real endpoints.
# Module 2: Static Data & Basic Endpoints

In this module, we will build our first functional endpoints without a database. This helps us focus on API structure, routing, and parameter binding.

## 1. Using Static Data (In-Memory)
Before connecting to a real database, let's simulate data using a static list. This is useful for prototyping.

**Scenario**: We want to manage a list of `Post` items.
First, define a simple `Post` class (Model) inline or in a `Models` folder.

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
```

Now, let's create a static list in our endpoint definitions.

## 2. Defining Endpoints
We will create a specific endpoint group for our static posts. This keeps `Program.cs` clean.

**File**: `Endpoints/StaticPostEndPoints.cs`

```csharp
public static class StaticPostEndPoints
{
    // Simulate a database with a static list
    internal static List<Post> posts = new()
    {
        new Post { Id = 1, Title = "First Post", Content = "Hello World" },
        new Post { Id = 2, Title = "Second Post", Content = "Minimal APIs are cool" }
    };

    public static RouteGroupBuilder MapStaticPost(this RouteGroupBuilder group)
    {
        // GET /staticpost
        group.MapGet("/", () => posts);

        // GET /staticpost/{id}
        group.MapGet("/{id}", (int id) =>
        {
            var post = posts.FirstOrDefault(p => p.Id == id);
            return post is not null ? Results.Ok(post) : Results.NotFound();
        })
        .WithName("GetStaticPostById");

        // POST /staticpost
        group.MapPost("/", (Post post) =>
        {
            post.Id = posts.Max(p => p.Id) + 1;
            posts.Add(post);
            return Results.Created($"/staticpost/{post.Id}", post);
        });

        // PUT /staticpost/{id}
        group.MapPut("/{id}", (int id, Post updatedPost) =>
        {
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post is null) return Results.NotFound();

            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;
            return Results.Ok(post);
        });

        // DELETE /staticpost/{id}
        group.MapDelete("/{id}", (int id) =>
        {
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post is null) return Results.NotFound();

            posts.Remove(post);
            return Results.NoContent();
        });

        return group;
    }
}
```

## 3. Registering the Group in Program.cs
To make these endpoints active, we must call the mapping method in `Program.cs`.

```csharp
// Program.cs
app.MapGroup("/staticpost")
   .MapStaticPost()
   .WithTags("StaticPostNews");
```

## 4. Parameter Binding
Minimal APIs automatically bind parameters.
*   **Route Values**: `/{id}` -> `(int id)`
*   **Body**: `(Post post)` (JSON body is deserialized)
*   **Query Strings**: `?term=abc` -> `(string term)`

> [!NOTE]
> We rely on the framework's conventions. Explicit attributes like `[FromRoute]` are rarely needed in .NET 8 Minimal APIs.

## 5. Swagger Customization
To make our API documentation professional, we can add a title, description, and contact info in `Program.cs` (or via extensions).

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Barefoot API",
        Description = "Built with .NET 8 Minimal API",
        Contact = new OpenApiContact
        {
            Name = "Alhafi BareFoot",
            Email = "alhafi@hotmail.com"
        }
    });
});
```

Result: A professional Swagger UI page greeting your users.
# Module 3: Database & Architecture

In this module, we transition from static lists to a real database using Entity Framework Core. We will also introduce the folder structure that defines **Clean Architecture**.

## 1. Clean Architecture Folders
We will organize our code into specific responsibilities:
*   `Data/Models`: The shape of our data (Entities).
*   `Data/`: The database context.
*   `Data/Migrations`: Database versioning history.
*   `Endpoints`: The API routes.

## 2. Defining the Model (Entity)
Let's create a robust `Post` model.

**File**: `Data/Models/Post.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace MinAPI.Data.Models;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; }

    public string? ImagePath { get; set; }
}
```

## 3. The Database Context
The `DbContext` is the bridge between our code and the database.

**File**: `Data/AppDbContext.cs`

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data.Models;

namespace MinAPI.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed default data
        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "Seed Post", Content = "Initial data from migration" }
        );
    }
}
```

## 4. Configuration & Dependency Injection
We need to tell the app to use Sqlite (or SQL Server) and where to find the connection string.

**File**: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=AlhafiNewsPaper.db"
  }
}
```

**File**: `Extensions/ServiceCollectionExtensions.cs` (or Program.cs)

```csharp
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 5. Migrations
Now we create the database schema from our code.

```powershell
# Create the initial migration
dotnet ef migrations add InitialCreate -o Data/Migrations

# Apply the migration to the database
dotnet ef database update
```

## 6. Using the Database in Endpoints
Instead of the static list, we now inject `AppDbContext` into our endpoints (later we will use Services/Repositories).

```csharp
group.MapGet("/", async (AppDbContext db) =>
    await db.Posts.ToListAsync());
```
# Module 4: Professional Patterns

Professional applications don't leak database models to the outside world. In this module, we introduce **DTOs (Data Transfer Objects)**, **Services**, and **AutoMapper**.

## 1. Why DTOs?
Exposing your database entity directly (`Post`) is risky. DTOs allow us to:
*   Hide internal fields (like `LastUpdated` or sensitive data).
*   Shape data specifically for the client.
*   Decouple the API contract from the database schema.

**File**: `Data/DTOs/PostDTOs.cs`

```csharp
public record PostDto(int Id, string Title, string Content, DateTime CreatedAt);

public record PostNewOrUpdatedDto(string Title, string Content);
```

## 2. AutoMapper Setup
Manually copying properties from Entity to DTO is tedious. `AutoMapper` automates this.

**File**: `Data/Profiles/PostProfile.cs`

```csharp
using AutoMapper;
using MinAPI.Data.DTOs;
using MinAPI.Data.Models;

namespace MinAPI.Data.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        // Source -> Destination
        CreateMap<Post, PostDto>();
        CreateMap<PostNewOrUpdatedDto, Post>();
    }
}
```

**Register AutoMapper** in `Extensions/ServiceCollectionExtensions.cs`:
```csharp
builder.Services.AddAutoMapper(typeof(Program));
```

## 3. The Service Layer
The Service layer holds our **Business Logic**. It sits between the API endpoint and the Data layer (Repository/DbContext).

**File**: `Services/Interfaces/IPostService.cs`
```csharp
public interface IPostService
{
    Task<IEnumerable<PostDto>> GetAllPosts();
    Task<PostDto?> GetPostById(int id);
    Task<PostDto> CreatePost(PostNewOrUpdatedDto newPost);
}
```

**File**: `Services/PostService.cs`
```csharp
public class PostService : IPostService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public PostService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PostDto>> GetAllPosts()
    {
        var posts = await _db.Posts.ToListAsync();
        return _mapper.Map<IEnumerable<PostDto>>(posts);
    }

    // ... Implement other methods
}
```

## 4. Updating Endpoints
Now our endpoints are clean and only talk to the Service.

```csharp
// Program.cs or Endpoints/AutoMapperPostEndPoints.cs
group.MapGet("/", async (IPostService service) =>
{
    var posts = await service.GetAllPosts();
    return Results.Ok(posts);
});
```

This pattern creates a separation of concerns that is essential for larger applications.

## 5. Advanced Dependency Injection
While Minimal APIs support implicit DI (just adding the type to the parameters), you can be explicit using `[FromServices]`.

**Example**: `Endpoints/DemoEndpoints.cs`
```csharp
group.MapGet("/FromRegisteredService", ([FromServices] IDateTime dateTime) =>
    dateTime.Now);
```

This is useful when you want to be clear about where a parameter is coming from, or when disambiguating between route parameters and services.
# Module 5: Data Operations (Search, Sort, Pagination)

As our data grows, we can't just return `GetAll()`. We need sophisticated querying capabilities.

## 1. Query Parameters Object
Instead of adding 10 parameters to our endpoint method `(string? search, int? page, ...)`, we create a dedicated object to bind query strings.

**File**: `Data/DTOs/PostQueryParameters.cs`
```csharp
public class PostQueryParameters
{
    public string? Sort { get; set; }
    public string? Search { get; set; }
    public string? Order { get; set; } = "asc";
    public int? Page { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
```

## 2. Parameter Binding in Endpoint
Minimal APIs are smart enough to map query string values to this object automatically.

```csharp
// GET /api/posts?search=api&sort=title&page=2
group.MapGet("/", async (IPostService service, [AsParameters] PostQueryParameters parameters) =>
{
    var posts = await service.GetAllPostsAsync(parameters);
    return Results.Ok(posts);
});
```

> [!TIP]
> `[AsParameters]` is the key here. It tells .NET to look for these properties in the query string.

## 3. Implementation in Repository
Now we need to apply this logic in our database query (Linq to Entities).

**File**: `Data/Repositories/PostRepo.cs`
```csharp
public async Task<IEnumerable<Post>> GetAllPosts(PostQueryParameters parameters)
{
    var query = _context.Posts.AsQueryable();

    // 1. Filtering (Search)
    if (!string.IsNullOrEmpty(parameters.Search))
    {
        var search = parameters.Search.ToLower();
        query = query.Where(p =>
            p.Title.ToLower().Contains(search) ||
            p.Content.ToLower().Contains(search));
    }

    // 2. Sorting
    query = parameters.Sort?.ToLower() switch
    {
        "title" => parameters.Order == "desc"
            ? query.OrderByDescending(p => p.Title)
            : query.OrderBy(p => p.Title),
        "content" => parameters.Order == "desc"
            ? query.OrderByDescending(p => p.Content)
            : query.OrderBy(p => p.Content),
        _ => query.OrderBy(p => p.Id) // Default sort
    };

    // 3. Pagination
    var page = parameters.Page ?? 1;
    var pageSize = parameters.PageSize ?? 10;

    return await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

This efficient query runs on the database server, returning only the requested page of data.

## 4. Output Caching
To improve performance, we can cache the results of expensive operations.

**Configuration**:
In `Program.cs` or `Extensions/ServiceCollectionExtensions.cs`, we define policies:
```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(60)));
    options.AddPolicy("PostCache", builder => builder.Expire(TimeSpan.FromDays(360)).Tag("Post_Get"));
});
```

**Usage**:
```csharp
app.MapGroup("/dbcontext")
   .MapDBConextPost()
   .CacheOutput("PostCache"); // Applies caching middleware
```
# Module 6: Robustness & Files

A professional API needs to handle errors gracefully, validate inputs before processing, and manage file uploads.

## 1. Global Error Handling
Instead of wrapping every controller action in a `try-catch` block, we use a global exception handler.

**File**: `Middlewares/GlobalExceptionHandler.cs`
```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = "An internal server error has occurred."
        };

        if (exception is ArgumentNullException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = "Invalid Argument";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
```

**Registering it**:
```csharp
// Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler(); // Adds the middleware
```

## 2. Validation with FluentValidation
We use **FluentValidation** to separate validation rules from the model.

**File**: `Validations/PostValidator.cs`
```csharp
public class PostNewOrUpdatedDtoValidator : AbstractValidator<PostNewOrUpdatedDto>
{
    public PostNewOrUpdatedDtoValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(25).WithMessage("Title max length is 25");
    }
}
```

**Applying Validation via Filter**:
We create a generic filter `ValidationFilter<T>` that intercepts requests and validates the body before the endpoint logic flows.

```csharp
// Endpoint Registration
group.MapPost("/", ...)
     .AddEndpointFilter<ValidationFilter<PostNewOrUpdatedDto>>();
```

## 3. Handling File Uploads
To upload files (images), we use `IFormFile` in our DTO.

**DTO Update**:
```csharp
public class PostNewOrUpdatedDto
{
    // ... other properties
    public IFormFile? Image { get; set; }
}
```

**Service Logic**:
The service saves the file to the `wwwroot/img/Posts` folder and updates the database path.

```csharp
// PostService.cs
if (postDto.Image != null)
{
    var fileName = $"Post{postModel.Id}_{postModel.Title}{Path.GetExtension(postDto.Image.FileName)}";
    var filePath = Path.Combine(_environment.WebRootPath, "img", "Posts", fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await postDto.Image.CopyToAsync(stream);
    }

    postModel.postImage = $"img/Posts/{fileName}";
    await _repo.SaveChanges();
}
```

This creates a complete flow handling data, validation, and binary files.

## 4. Testing Robustness
Your API includes specific endpoints to simulate failures and verify your global error handler is working.

**Test Endpoints** (`/api/test/`):
*   `GET /not-found/{id}`: value **1** returns 404.
*   `GET /database-error`: Throws a `DatabaseException`.
*   `GET /unauthorized`: Returns 401.

Use these in Swagger to demonstrate to students how the API reacts to problems.
# Module 7: Security (Auth & CORS)

Security is paramount. We will implement **JWT (JSON Web Token)** authentication and configure **CORS** for frontend access.

## 1. Authentication Service
The `AuthService` handles user registration and login, issuing JWT tokens upon success.

**File**: `Services/AuthService.cs`
```csharp
public async Task<UserDto?> LoginAsync(LoginDto loginDto)
{
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    // ... Verify password ...

    return new UserDto
    {
        Id = user.Id,
        Email = user.Email!,
        Token = GenerateJwtToken(user) // Issue the token
    };
}
```

## 2. JWT Configuration (Program.cs)
We need to validate the tokens sent by users.

**File**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
```

### Pro Tip: The "Magic" Developer Token
For development purposes, we have implemented a backdoor to easily generate tokens without a full login flow.

**How it works**:
If you send a request with the header `Authorization: Bearer barefoot2020`, the API intercepts this in the `OnMessageReceived` event and generates a valid "Dev Token" on the fly.

**Code Reference**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
OnMessageReceived = context =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (token == "barefoot2020")
    {
        context.Token = GenerateDevToken(builder.Configuration);
    }
    return Task.CompletedTask;
}
```
> [!WARNING]
> Ensure this logic is removed or disabled in Production environments!

## 3. Securing Endpoints
Now we can protect any endpoint using `RequireAuthorization()`.

```csharp
group.MapPost("/", ...)
     .RequireAuthorization(); // 🔒 Locked!
```

## 4. Swagger Security Definition
To test this in Swagger, we need to add the "Authorize" button.

**File**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Description = "Enter your token in the text input below.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT"
});
c.OperationFilter<SecurityRequirementsOperationFilter>();
```
The `SecurityRequirementsOperationFilter` automatically adds the lock icon to protected endpoints in the UI.

## 5. CORS (Cross-Origin Resource Sharing)
If your frontend (React/Angular) calls this API from a different port, you must enable CORS.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ...
app.UseCors("MyAllowedOrigins");
```

---
# 🎉 Conclusion
Congratulations! You have built a production-grade Minimal API with Clean Architecture, Database, Validation, and Security.
