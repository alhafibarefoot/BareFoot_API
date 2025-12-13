# Module 6: Robustness - Validation & Error Handling

Building robust gRPC services requires proper validation and error handling. In gRPC, **Interceptors** serve as middleware for cross-cutting concerns.

## 🎯 What are Interceptors?

Interceptors are like middleware in ASP.NET Core - they intercept RPC calls before they reach your service methods.

### Use Cases:
- ✅ **Validation**: Check request data before processing
- ✅ **Logging**: Log all requests and responses
- ✅ **Error Handling**: Catch and transform exceptions
- ✅ **Authentication**: Verify tokens
- ✅ **Performance Monitoring**: Track execution time

## 🛡️ Error Handling with RpcException

Unlike REST APIs that use HTTP status codes, gRPC uses `RpcException` with specific status codes.

### Common Status Codes:

| Status Code | HTTP Equivalent | Use Case |
|-------------|----------------|----------|
| `OK` | 200 | Success |
| `Cancelled` | 499 | Client cancelled |
| `InvalidArgument` | 400 | Bad request data |
| `NotFound` | 404 | Resource not found |
| `AlreadyExists` | 409 | Duplicate resource |
| `PermissionDenied` | 403 | Authorization failed |
| `Unauthenticated` | 401 | Not authenticated |
| `ResourceExhausted` | 429 | Rate limit exceeded |
| `Internal` | 500 | Server error |
| `Unavailable` | 503 | Service unavailable |

### Basic Error Handling:

```csharp
public override async Task<PostModel> ReadPost(
    ReadPostRequest request,
    ServerCallContext context)
{
    if (request.Id <= 0)
    {
        throw new RpcException(new Status(
            StatusCode.InvalidArgument,
            "Post ID must be greater than 0"));
    }

    var post = await _context.Posts.FindAsync(request.Id);

    if (post == null)
    {
        throw new RpcException(new Status(
            StatusCode.NotFound,
            $"Post with ID {request.Id} not found"));
    }

    return _mapper.Map<PostModel>(post);
}
```

## 🔧 Creating an Exception Interceptor

Interceptors catch exceptions globally and convert them to proper `RpcException` responses.

### 1. Create the Interceptor

**Interceptors/ExceptionInterceptor.cs:**

```csharp
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace gRPC.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (RpcException)
            {
                // Already an RpcException, just rethrow
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument");
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                throw new RpcException(new Status(
                    StatusCode.PermissionDenied,
                    "Access denied"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "An internal error occurred"));
            }
        }
    }
}
```

### 2. Register the Interceptor

**Program.cs:**

```csharp
using gRPC.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

var app = builder.Build();
```

## ✅ Validation with FluentValidation

FluentValidation provides a clean way to validate request data.

### 1. Install FluentValidation

```powershell
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### 2. Create Validators

**Validators/CreatePostRequestValidator.cs:**

```csharp
using FluentValidation;

namespace gRPC.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters");
        }
    }
}
```

### 3. Create Validation Interceptor

**Interceptors/ValidationInterceptor.cs:**

```csharp
using Grpc.Core;
using Grpc.Core.Interceptors;
using FluentValidation;

namespace gRPC.Interceptors
{
    public class ValidationInterceptor : Interceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            // Try to get validator for this request type
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();

            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        $"Validation failed: {errors}"));
                }
            }

            return await continuation(request, context);
        }
    }
}
```

### 4. Register Validators

**Program.cs:**

```csharp
using FluentValidation;
using gRPC.Validators;

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ValidationInterceptor>();
    options.Interceptors.Add<ExceptionInterceptor>();
});

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<CreatePostRequestValidator>();
```

## 📊 Logging Interceptor

Track all requests and responses for debugging and monitoring.

**Interceptors/LoggingInterceptor.cs:**

```csharp
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace gRPC.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();
            var methodName = context.Method;

            _logger.LogInformation("Starting call: {Method}", methodName);

            try
            {
                var response = await continuation(request, context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Completed call: {Method} in {ElapsedMs}ms",
                    methodName,
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Failed call: {Method} in {ElapsedMs}ms",
                    methodName,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
```

## 🔄 Interceptor Order

The order matters! Interceptors are executed in the order they're registered:

```csharp
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();      // 1. Log first
    options.Interceptors.Add<ValidationInterceptor>();   // 2. Validate
    options.Interceptors.Add<ExceptionInterceptor>();    // 3. Handle errors
});
```

Execution flow:
```
Request → Logging → Validation → Exception → Service → Exception → Validation → Logging → Response
```

## 🎯 Best Practices

### Error Handling:
1. **Use specific status codes** - Don't always return `Internal`
2. **Don't expose sensitive info** - Keep error messages generic for clients
3. **Log detailed errors** - But only on the server side
4. **Use interceptors** - For consistent error handling

### Validation:
1. **Validate early** - Before hitting the database
2. **Use FluentValidation** - Clean and testable
3. **Return meaningful messages** - Help clients fix issues
4. **Validate in interceptors** - Keep services clean

### Interceptors:
1. **Keep them focused** - One responsibility per interceptor
2. **Order matters** - Think about the execution flow
3. **Use DI** - Inject services as needed
4. **Handle both success and failure** - In logging/monitoring interceptors

## 🔍 Testing Error Handling

### Client-side Error Handling:

```csharp
try
{
    var post = await client.ReadPostAsync(new ReadPostRequest { Id = 999 });
}
catch (RpcException ex)
{
    switch (ex.StatusCode)
    {
        case StatusCode.NotFound:
            Console.WriteLine("Post not found");
            break;
        case StatusCode.InvalidArgument:
            Console.WriteLine($"Invalid request: {ex.Status.Detail}");
            break;
        case StatusCode.Internal:
            Console.WriteLine("Server error occurred");
            break;
        default:
            Console.WriteLine($"Error: {ex.Status.Detail}");
            break;
    }
}
```

## 🎓 Summary

In this module, you learned:
- ✅ How to use `RpcException` with proper status codes
- ✅ Creating interceptors for cross-cutting concerns
- ✅ Global exception handling with interceptors
- ✅ Validation using FluentValidation
- ✅ Logging and performance monitoring
- ✅ Interceptor execution order and best practices

## 📚 Next Steps

Continue to [Module 7: Security](module_07.md) to learn about JWT authentication and authorization in gRPC.
