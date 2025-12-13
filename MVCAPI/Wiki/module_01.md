# Module 1: Fundamentals & Setup

In this module, we will establish the foundation of our Web API (MVC controller-based) project. We'll start from scratch, understand the project structure, and get our first Controller running.

## 1. Create the Project
We will use the .NET CLI to create a new Web API (MVC controller-based) project.

```powershell
# Create a new Web API project
dotnet new webapi -minimal -n MVCAPI

# Navigate into the project directory
cd MVCAPI
```

> [!TIP]
> The `-minimal` flag ensures we get the lightweight Web API (MVC controller-based) template without Controllers.

**Note on JSON**: This project includes `Microsoft.AspNetCore.Mvc.NewtonsoftJson`. While .NET 9 uses `System.Text.Json` by default, Newtonsoft is often added for specific serialization behaviors or legacy compatibility.

## 2. The `.gitignore` File
Git is essential. A proper `.gitignore` prevents clutter (like `bin/`, `obj/`, `user secrets`) from hitting your repository.

**Create a `.gitignore`** at the root of your solution with standard .NET content:

```gitignore
# Build Results
[bB]in/
[oO]bj/

# IDEs
.vs/
.vscode/
.idea/
*.user
*.suo

# Databases & Logs
*.db
*.sqlite
*.log

# Web
node_modules/
appsettings.Development.json
```

## 3. Project Structure
Your new project looks like this. Let's create the folder structure we will need effectively implementing **Clean Architecture** later.

```text
MVCAPI/
├── Program.cs          # Entry point & Configuration
├── MVCAPI.csproj       # Project file & Dependencies
├── appsettings.json    # Configuration (Connections, Logging)
│
├── Data/               # Database Context & Repositories
├── Data/Models/        # Entity Models
├── Controllers/          # API Route Groups (Controllers)
├── Extensions/         # Extension methods for clean Program.cs
├── Services/           # Business Logic
├── DTOs/               # Data Transfer Objects
└── Validations/        # Input Validation Rules
```

> [!IMPORTANT]
> Creating these folders now helps us stay organized as the project grows.

## 4. Understanding `Program.cs`
In Web API (MVC controller-based)s, `Program.cs` is where everything happens.
It has three main zones:

1.  **Builder & Services Zone**: Registration of DI (Dependency Injection) containers.
2.  **Build**: The app is built.
3.  **Middleware & Pipelines**: Request handling pipeline (Services, Swaggers, Https).

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES ZONE
// Add services to the container.
builder.Services.AddControllersApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. MIDDLEWARE ZONE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. Controllers ZONE
app.MapGet("/weatherforecast", () => { ... })
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
```

## Next Step
Now that we have the project created, let's start adding **Static Data** and building real Controllers.
