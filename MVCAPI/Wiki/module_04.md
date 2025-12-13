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
using MVCAPI.Data.DTOs;
using MVCAPI.Data.Models;

namespace MVCAPI.Data.Profiles;

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
The Service layer holds our **Business Logic**. It sits between the API Controller and the Data layer (Repository/DbContext).

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

## 4. Updating Controllers
Now our Controllers are clean and only talk to the Service.

```csharp
// Program.cs or Controllers/AutoMapperPostControllers.cs
group.MapGet("/", async (IPostService service) =>
{
    var posts = await service.GetAllPosts();
    return Results.Ok(posts);
});
```

This pattern creates a separation of concerns that is essential for larger applications.

## 5. Advanced Dependency Injection
While Web API (MVC controller-based)s support implicit DI (just adding the type to the parameters), you can be explicit using `[FromServices]`.

**Example**: `Controllers/DemoControllers.cs`
```csharp
group.MapGet("/FromRegisteredService", ([FromServices] IDateTime dateTime) =>
    dateTime.Now);
```

This is useful when you want to be clear about where a parameter is coming from, or when disambiguating between route parameters and services.
