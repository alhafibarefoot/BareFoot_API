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
> We rely on the framework's conventions. Explicit attributes like `[FromRoute]` are rarely needed in .NET 9 Minimal APIs.

## 5. Swagger Customization
To make our API documentation professional, we can add a title, description, and contact info in `Program.cs` (or via extensions).

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Barefoot API",
        Description = "Built with .NET 9 Minimal API",
        Contact = new OpenApiContact
        {
            Name = "Alhafi BareFoot",
            Email = "alhafi@hotmail.com"
        }
    });
});
```

Result: A professional Swagger UI page greeting your users.
