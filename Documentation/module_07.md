# Module 7: Security (Auth & CORS)

Security is paramount. We will implement **JWT (JSON Web Token)** authentication and configure **CORS** for frontend access.

## 1. Authentication Service
The `AuthService` handles user registration and login, issuing JWT tokens upon success.

**File**: `Services/AuthService.cs`
```csharp
public async Task<UserDto?> LoginAsync(LoginDto loginDto)
{
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    // ... Verify password ...

    return new UserDto
    {
        Id = user.Id,
        Email = user.Email!,
        Token = GenerateJwtToken(user) // Issue the token
    };
}
```

## 2. JWT Configuration (Program.cs)
We need to validate the tokens sent by users.

**File**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
```

### Pro Tip: The "Magic" Developer Token
For development purposes, we have implemented a backdoor to easily generate tokens without a full login flow.

**How it works**:
If you send a request with the header `Authorization: Bearer barefoot2020`, the API intercepts this in the `OnMessageReceived` event and generates a valid "Dev Token" on the fly.

**Code Reference**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
OnMessageReceived = context =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (token == "barefoot2020")
    {
        context.Token = GenerateDevToken(builder.Configuration);
    }
    return Task.CompletedTask;
}
```
> [!WARNING]
> Ensure this logic is removed or disabled in Production environments!

## 3. Securing Endpoints
Now we can protect any endpoint using `RequireAuthorization()`.

```csharp
group.MapPost("/", ...)
     .RequireAuthorization(); // 🔒 Locked!
```

## 4. Swagger Security Definition
To test this in Swagger, we need to add the "Authorize" button.

**File**: `Extensions/ServiceCollectionExtensions.cs`
```csharp
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Description = "Enter your token in the text input below.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT"
});
c.OperationFilter<SecurityRequirementsOperationFilter>();
```
The `SecurityRequirementsOperationFilter` automatically adds the lock icon to protected endpoints in the UI.

## 5. CORS (Cross-Origin Resource Sharing)
If your frontend (React/Angular) calls this API from a different port, you must enable CORS.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ...
app.UseCors("MyAllowedOrigins");
```

---
# 🎉 Conclusion
Congratulations! You have built a production-grade Minimal API with Clean Architecture, Database, Validation, and Security.
