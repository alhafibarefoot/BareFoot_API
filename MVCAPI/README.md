# BareFoot API (MVCAPI)


> [!NOTE]
> 🎧 **Listen to the interactive audio course on [Google NotebookLM](https://notebooklm.google.com/notebook/b1e53748-2a0c-42e8-9fca-36210af3f31b)**

A robust, "Clean Architecture" inspired .NET 9 Web API (MVC controller-based) project. This API provides news post management with advanced features like output caching, global error handling, and a dedicated service layer.

## 🚀 Key Features

### 🏛️ Architecture
-   **Service Layer Pattern**: Business logic is encapsulated in `MVCAPI.Services`, separating concerns from Controllers.
-   **Structured Folders**:
    -   `Controllers/`: functional slices of API routes.
    -   `Extensions/`: Dependency Injection and Middleware configurations.
    -   `Services/`: Business logic implementations.
    -   `Data/`: EF Core context and Repositories.

### ✨ Advanced Capabilities
-   **Filtering, Sorting & Pagination**: The `/dbcontext/posts` Controller supports dynamic querying:
    -   `search`: Filter by title or content.
    -   `sort` & `order`: Sort by ID, Title, or Content (asc/desc).
    -   `page` & `pageSize`: Efficient data retrieval.
-   **Output Caching**: High-performance caching using named policies (e.g., `PostCache` expires in 360 days).
-   **Global Error Handling**: Centralized handling via `GlobalExceptionHandler` returning RFC 7807 problem details.
-   **Security**: Integrated with **ASP.NET Core Identity** and **JWT Bearer Authentication**. Protected Controllers require valid tokens.
-   **Test Controllers**: A suite of simulation Controllers at `/api/test/` to verify system behavior under failure conditions.

## 🛠️ Technology Stack
-   **.NET 9** Web API (MVC controller-based)
-   **ASP.NET Core Identity & JWT**
-   **Entity Framework Core** (SQLite)
-   **AutoMapper** for DTO mapping
-   **FluentValidation**
-   **Swagger/OpenAPI** with XML comments

## 🏁 Getting Started

### Prerequisites
-   .NET 9 SDK

### Build and Run
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

Access the API documentation at:
**https://localhost:7010/swagger**

## 📂 Project Structure

```
MVCAPI/
├── Data/               # DB Context and Repositories
├── Controllers/          # API Route Definitions
│   ├── DBConextPostControllers.cs
│   ├── TestControllers.cs
│   └── ...
├── Extensions/         # Service & Middleware Config
├── Middlewares/        # Global Exception Handler
├── Services/           # Business Logic (PostService)
├── Validations/        # FluentValidation rules
└── Program.cs          # Entry point
```

## 🧪 Testing
You can verify error handling by visiting the `Testing` section in Swagger or calling:
-   `/api/test/not-found/1`
-   `/api/test/database-error`
-   `/api/test/unauthorized`
