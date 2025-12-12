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
