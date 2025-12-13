# Module 7: Security - Authentication & Authorization

Securing your gRPC services is crucial for production applications. This module covers JWT authentication, authorization, and TLS/SSL configuration.

## 🎯 Security Overview

### Key Concepts:
- **Authentication**: Verifying who the user is (JWT tokens)
- **Authorization**: Verifying what the user can do (roles, policies)
- **Transport Security**: Encrypting data in transit (TLS/SSL)
- **API Keys**: Simple authentication for service-to-service calls

## 🔐 JWT Authentication

JWT (JSON Web Tokens) is the standard for securing gRPC services.

### 1. Install Required Packages

```powershell
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

### 2. Configure JWT Authentication

**appsettings.json:**

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "ExpiryInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gRPC.db"
  }
}
```

**Program.cs:**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC
builder.Services.AddGrpc();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<PostService>();

app.Run();
```

### 3. Protect Services with [Authorize]

**Services/PostService.cs:**

```csharp
using Microsoft.AspNetCore.Authorization;
using Grpc.Core;

namespace gRPC.Services
{
    [Authorize] // Require authentication for all methods
    public class PostService : gRPC.PostService.PostServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostService> _logger;

        public PostService(AppDbContext context, ILogger<PostService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // This method requires authentication
        public override async Task<PostModel> CreatePost(
            CreatePostRequest request,
            ServerCallContext context)
        {
            // Get user from context
            var userId = context.GetHttpContext().User.FindFirst("sub")?.Value;
            _logger.LogInformation("User {UserId} creating post", userId);

            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                CreatedOn = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return _mapper.Map<PostModel>(post);
        }

        [AllowAnonymous] // Allow this method without authentication
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
}
```

### 4. Create Token Generation Service

**Services/ITokenService.cs:**

```csharp
public interface ITokenService
{
    string GenerateToken(string userId, string username, List<string> roles);
}
```

**Services/TokenService.cs:**

```csharp
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace gRPC.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string username, List<string> roles)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiryInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
```

**Register in Program.cs:**

```csharp
builder.Services.AddSingleton<ITokenService, TokenService>();
```

### 5. Create Auth Service (Optional)

**Protos/auth.proto:**

```protobuf
syntax = "proto3";

option csharp_namespace = "gRPC";

package auth;

service AuthService {
  rpc Login (LoginRequest) returns (LoginResponse);
}

message LoginRequest {
  string username = 1;
  string password = 2;
}

message LoginResponse {
  string token = 1;
  string expires_at = 2;
}
```

**Services/AuthService.cs:**

```csharp
using Grpc.Core;

namespace gRPC.Services
{
    public class AuthService : gRPC.AuthService.AuthServiceBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ITokenService tokenService, ILogger<AuthService> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public override Task<LoginResponse> Login(
            LoginRequest request,
            ServerCallContext context)
        {
            // TODO: Validate credentials against database
            // This is a simplified example
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _tokenService.GenerateToken(
                    "1",
                    request.Username,
                    new List<string> { "Admin" });

                return Task.FromResult(new LoginResponse
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(1).ToString("o")
                });
            }

            throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                "Invalid credentials"));
        }
    }
}
```

## 🔑 Client-Side: Sending JWT Tokens

### Using Metadata Headers:

```csharp
using Grpc.Core;
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:7021");
var client = new PostService.PostServiceClient(channel);

// Get token (from login)
var authClient = new AuthService.AuthServiceClient(channel);
var loginResponse = await authClient.LoginAsync(new LoginRequest
{
    Username = "admin",
    Password = "password"
});

var token = loginResponse.Token;

// Create metadata with token
var headers = new Metadata
{
    { "Authorization", $"Bearer {token}" }
};

// Call protected method with token
var post = await client.CreatePostAsync(
    new CreatePostRequest
    {
        Title = "Authenticated Post",
        Content = "This requires auth"
    },
    headers);
```

### Using CallCredentials (Recommended):

```csharp
var credentials = CallCredentials.FromInterceptor((context, metadata) =>
{
    metadata.Add("Authorization", $"Bearer {token}");
    return Task.CompletedTask;
});

var channel = GrpcChannel.ForAddress("https://localhost:7021", new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create(
        new SslCredentials(),
        credentials)
});
```

## 👥 Role-Based Authorization

### Using [Authorize] with Roles:

```csharp
[Authorize(Roles = "Admin")]
public override async Task<PostModel> DeletePost(
    DeletePostRequest request,
    ServerCallContext context)
{
    // Only admins can delete
    var post = await _context.Posts.FindAsync(request.Id);
    if (post == null)
    {
        throw new RpcException(new Status(StatusCode.NotFound, "Post not found"));
    }

    _context.Posts.Remove(post);
    await _context.SaveChangesAsync();

    return _mapper.Map<PostModel>(post);
}
```

### Using Policy-Based Authorization:

**Program.cs:**

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequirePostOwner", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "PostOwner")));
});
```

**Service:**

```csharp
[Authorize(Policy = "RequireAdminRole")]
public override async Task<AdminResponse> AdminOnlyMethod(...)
{
    // Implementation
}
```

## 🔒 TLS/SSL Configuration

gRPC **requires** HTTPS in production. Development certificates are auto-configured.

### Trust Development Certificate:

```powershell
dotnet dev-certs https --trust
```

### Production Configuration:

**appsettings.Production.json:**

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://*:443",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate-password"
        }
      }
    }
  }
}
```

## 🛡️ Security Best Practices

### ✅ Do:
1. **Always use HTTPS/TLS** in production
2. **Store secrets securely** - Use Azure Key Vault, AWS Secrets Manager, etc.
3. **Validate tokens** - Check issuer, audience, expiry
4. **Use strong keys** - At least 256 bits for JWT signing
5. **Implement rate limiting** - Prevent abuse
6. **Log security events** - Track authentication failures
7. **Use short token expiry** - Refresh tokens for long sessions

### ❌ Don't:
1. **Don't store secrets in code** - Use configuration/environment variables
2. **Don't use weak keys** - Avoid simple passwords
3. **Don't expose detailed errors** - Keep error messages generic
4. **Don't skip HTTPS** - Even in development, use it
5. **Don't trust client data** - Always validate on server

## 🔍 Testing Authentication

### Test with Invalid Token:

```csharp
try
{
    var headers = new Metadata
    {
        { "Authorization", "Bearer invalid-token" }
    };

    await client.CreatePostAsync(new CreatePostRequest { ... }, headers);
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
{
    Console.WriteLine("Authentication failed as expected");
}
```

### Test with Missing Token:

```csharp
try
{
    await client.CreatePostAsync(new CreatePostRequest { ... });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
{
    Console.WriteLine("No token provided - access denied");
}
```

## 🎓 Summary

In this module, you learned:
- ✅ How to configure JWT authentication in gRPC
- ✅ Protecting services with `[Authorize]` attribute
- ✅ Generating and validating JWT tokens
- ✅ Sending tokens from gRPC clients
- ✅ Role-based and policy-based authorization
- ✅ TLS/SSL configuration for secure communication
- ✅ Security best practices for production

## 🎉 Congratulations!

You've completed all 7 modules of the gRPC course! You now have the knowledge to build high-performance, secure, and robust gRPC services in .NET 9.

### Next Steps:
- Build a real-world project using gRPC
- Explore advanced topics like load balancing and service mesh
- Check out the [main README](../README.md) for more resources
- Try the [MinAPI](../../MinAPI/Wiki/Home.md) and [MVCAPI](../../MVCAPI/Wiki/Home.md) courses
