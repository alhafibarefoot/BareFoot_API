# Module 3: Database & Architecture

In this module, we transition from static lists to a real database using Entity Framework Core. We will also introduce the folder structure that defines **Clean Architecture**.

## 1. Clean Architecture Folders
We will organize our code into specific responsibilities:
*   `Data/Models`: The shape of our data (Entities).
*   `Data/`: The database context.
*   `Data/Migrations`: Database versioning history.
*   `Controllers`: The API routes.

## 2. Defining the Model (Entity)
Let's create a robust `Post` model.

**File**: `Data/Models/Post.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace MVCAPI.Data.Models;

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
using MVCAPI.Data.Models;

namespace MVCAPI.Data;

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

## 6. Using the Database in Controllers
Instead of the static list, we now inject `AppDbContext` into our Controllers (later we will use Services/Repositories).

```csharp
group.MapGet("/", async (AppDbContext db) =>
    await db.Posts.ToListAsync());
```
