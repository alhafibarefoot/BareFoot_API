# BareFoot API (MinAPI)

A robust, "Clean Architecture" inspired .NET 8 Minimal API project. This API provides news post management with advanced features like output caching, global error handling, and a dedicated service layer.

## 🚀 Key Features

### 🏛️ Architecture
-   **Service Layer Pattern**: Business logic is encapsulated in `MinAPI.Services`, separating concerns from Endpoints.
-   **Structured Folders**:
    -   `Endpoints/`: functional slices of API routes.
    -   `Extensions/`: Dependency Injection and Middleware configurations.
    -   `Services/`: Business logic implementations.
    -   `Data/`: EF Core context and Repositories.

### ✨ Advanced Capabilities
-   **Filtering, Sorting & Pagination**: The `/dbcontext/posts` endpoint supports dynamic querying:
    -   `search`: Filter by title or content.
    -   `sort` & `order`: Sort by ID, Title, or Content (asc/desc).
    -   `page` & `pageSize`: Efficient data retrieval.
-   **Output Caching**: High-performance caching using named policies (e.g., `PostCache` expires in 360 days).
-   **Global Error Handling**: Centralized handling via `GlobalExceptionHandler` returning RFC 7807 problem details.
-   **Security**: Integrated with **ASP.NET Core Identity** and **JWT Bearer Authentication**. Protected endpoints require valid tokens.
-   **Test Endpoints**: A suite of simulation endpoints at `/api/test/` to verify system behavior under failure conditions.

## 🛠️ Technology Stack
-   **.NET 8** Minimal API
-   **ASP.NET Core Identity & JWT**
-   **Entity Framework Core** (SQLite)
-   **AutoMapper** for DTO mapping
-   **FluentValidation**
-   **Swagger/OpenAPI** with XML comments

## 🏁 Getting Started

### Prerequisites
-   .NET 8 SDK

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
MinAPI/
├── Data/               # DB Context and Repositories
├── Endpoints/          # API Route Definitions
│   ├── DBConextPostEndPoints.cs
│   ├── TestEndpoints.cs
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
