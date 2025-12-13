# Module 5: Data Operations (Search, Sort, Pagination)

As our data grows, we can't just return `GetAll()`. We need sophisticated querying capabilities.

## 1. Query Parameters Object
Instead of adding 10 parameters to our Controller method `(string? search, int? page, ...)`, we create a dedicated object to bind query strings.

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

## 2. Parameter Binding in Controller
Web API (MVC controller-based)s are smart enough to map query string values to this object automatically.

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
app.MapControllers("/dbcontext")
   .MapDBConextPost()
   .CacheOutput("PostCache"); // Applies caching middleware
```
