# Module 4: Professional Patterns

Decoupling your internal Entities from your external Protobuf Messages is crucial for maintainability, security, and flexibility.

## 🎯 Why Separate Entities from Messages?

### Problems with Direct Entity Exposure:
- ❌ **Security**: Exposes internal database structure
- ❌ **Coupling**: Changes to DB require API changes
- ❌ **Over-fetching**: Sends unnecessary data
- ❌ **Breaking Changes**: Hard to version the API

### Benefits of DTOs (Protobuf Messages):
- ✅ **Abstraction**: Hide internal implementation
- ✅ **Flexibility**: Change DB without breaking API
- ✅ **Security**: Control what data is exposed
- ✅ **Versioning**: Easier to maintain backward compatibility

## 📦 DTO Pattern in gRPC

In gRPC, **Protobuf messages ARE your DTOs**. They serve the same purpose as DTOs in REST APIs.

### The Pattern:

```
Database Entity → Protobuf Message → Client
     ↓                  ↑
   (Map)            (Return)
```

### Example:

**Entity (Internal):**
```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsDeleted { get; set; }  // Internal flag
}
```

**Protobuf Message (External):**
```protobuf
message PostModel {
  int32 id = 1;
  string title = 2;
  string content = 3;
  string created_on = 4;
  // Note: IsDeleted is NOT exposed
}
```

## 🗺️ Manual Mapping

The simplest approach is manual mapping:

```csharp
public override async Task<PostModel> ReadPost(
    ReadPostRequest request,
    ServerCallContext context)
{
    var post = await _context.Posts.FindAsync(request.Id);

    if (post == null)
    {
        throw new RpcException(new Status(StatusCode.NotFound, "Post not found"));
    }

    // Manual mapping
    return new PostModel
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        CreatedOn = post.CreatedOn.ToString("o")
    };
}
```

### Pros & Cons:
- ✅ Simple and explicit
- ✅ No dependencies
- ❌ Repetitive code
- ❌ Error-prone for complex objects

## 🔄 AutoMapper Integration

AutoMapper automates the mapping process.

### 1. Install AutoMapper

```powershell
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

### 2. Create Mapping Profile

**Mappings/MappingProfile.cs:**

```csharp
using AutoMapper;
using gRPC.Data.Models;

namespace gRPC.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to Protobuf Message
            CreateMap<Post, PostModel>()
                .ForMember(dest => dest.CreatedOn,
                    opt => opt.MapFrom(src => src.CreatedOn.ToString("o")));

            // Protobuf Message to Entity (for updates)
            CreateMap<CreatePostRequest, Post>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore());
        }
    }
}
```

### 3. Register AutoMapper

**Program.cs:**

```csharp
using gRPC.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(...);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
```

### 4. Use in Service

```csharp
using AutoMapper;

public class PostService : gRPC.PostService.PostServiceBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PostService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public override async Task<PostModel> ReadPost(
        ReadPostRequest request,
        ServerCallContext context)
    {
        var post = await _context.Posts.FindAsync(request.Id);

        if (post == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Post not found"));
        }

        // AutoMapper does the mapping
        return _mapper.Map<PostModel>(post);
    }

    public override async Task<ListPostsReply> ListPosts(
        ListPostsRequest request,
        ServerCallContext context)
    {
        var posts = await _context.Posts.ToListAsync();

        var reply = new ListPostsReply();
        reply.Posts.AddRange(_mapper.Map<List<PostModel>>(posts));

        return reply;
    }
}
```

## 🏗️ Service Layer Pattern

For complex business logic, add a service layer between gRPC services and the database.

### Architecture:

```
gRPC Service → Business Service → Repository → Database
   (API)         (Logic)           (Data)
```

### Example:

**Services/IPostBusinessService.cs:**

```csharp
public interface IPostBusinessService
{
    Task<Post> CreatePostAsync(string title, string content);
    Task<Post?> GetPostByIdAsync(int id);
    Task<List<Post>> GetAllPostsAsync();
}
```

**Services/PostBusinessService.cs:**

```csharp
public class PostBusinessService : IPostBusinessService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PostBusinessService> _logger;

    public PostBusinessService(AppDbContext context, ILogger<PostBusinessService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Post> CreatePostAsync(string title, string content)
    {
        // Business logic here
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty");
        }

        var post = new Post
        {
            Title = title,
            Content = content,
            CreatedOn = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created post: {PostId}", post.Id);
        return post;
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _context.Posts.ToListAsync();
    }
}
```

**Register in Program.cs:**

```csharp
builder.Services.AddScoped<IPostBusinessService, PostBusinessService>();
```

**Use in gRPC Service:**

```csharp
public class PostService : gRPC.PostService.PostServiceBase
{
    private readonly IPostBusinessService _businessService;
    private readonly IMapper _mapper;

    public PostService(IPostBusinessService businessService, IMapper mapper)
    {
        _businessService = businessService;
        _mapper = mapper;
    }

    public override async Task<PostModel> CreatePost(
        CreatePostRequest request,
        ServerCallContext context)
    {
        var post = await _businessService.CreatePostAsync(
            request.Title,
            request.Content);

        return _mapper.Map<PostModel>(post);
    }
}
```

## 🎯 Best Practices

### Mapping:
1. **Always map** - Never expose entities directly
2. **Use AutoMapper** for complex mappings
3. **Manual mapping** is fine for simple cases
4. **Map in the service** - Keep mapping logic centralized

### Service Layer:
1. **Use when needed** - Don't over-engineer simple CRUD
2. **Business logic** belongs in the service layer
3. **Validation** should happen in the service layer
4. **Keep gRPC services thin** - They should just orchestrate

### Naming:
- **Entities**: `Post`, `User`, `Comment`
- **Protobuf Messages**: `PostModel`, `UserModel`, `CommentModel`
- **Requests**: `CreatePostRequest`, `UpdatePostRequest`
- **Replies**: `ListPostsReply`, `SearchReply`

## 🎓 Summary

In this module, you learned:
- ✅ Why to separate entities from Protobuf messages
- ✅ Manual mapping vs AutoMapper
- ✅ How to configure and use AutoMapper
- ✅ Service layer pattern for complex logic
- ✅ Best practices for professional gRPC architecture

## 📚 Next Steps

Continue to [Module 5: Streaming](module_05.md) to learn about high-performance streaming in gRPC.
