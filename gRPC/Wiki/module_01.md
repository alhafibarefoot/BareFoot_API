# Module 1: Fundamentals & Setup

In this module, we will establish the foundation of our gRPC API project and understand the core concepts.

## 🎯 What is gRPC?

**gRPC** (gRPC Remote Procedure Call) is a modern, high-performance framework developed by Google. It allows you to call methods on a server application from a client as if it were a local object.

### Key Advantages:
- **Performance**: Uses HTTP/2 and binary serialization (Protobuf) - up to 7x faster than REST
- **Type Safety**: Strongly-typed contracts prevent runtime errors
- **Code Generation**: Automatic client/server code generation from `.proto` files
- **Streaming**: Built-in support for real-time data streaming
- **Cross-Platform**: Works across languages (C#, Java, Python, Go, etc.)

## 1. Prerequisites

Before starting, ensure you have:
- **.NET 9 SDK** installed
- A code editor (Visual Studio, VS Code, or Rider)
- Basic understanding of C# and async/await

Verify your .NET installation:
```powershell
dotnet --version
# Should show 9.0.x or higher
```

## 2. Create the Project

Use the .NET CLI to create a new gRPC project:

```powershell
dotnet new grpc -n gRPC
cd gRPC
```

This creates a project with:
- A sample `Greeter` service
- Default `.proto` file
- Basic configuration

## 3. Project Structure

gRPC projects have a unique structure compared to REST APIs:

```text
gRPC/
├── Protos/                    # Protocol Buffer definitions (.proto files)
│   ├── greet.proto           # Service contracts and message definitions
│   └── post.proto            # Additional service contracts
├── Services/                  # C# implementations of proto services
│   ├── GreeterService.cs     # Implements Greeter service
│   └── PostService.cs        # Implements Post CRUD service
├── Data/                      # Database layer (if using EF Core)
│   ├── AppDbContext.cs       # Entity Framework context
│   └── Models/               # Entity models
├── Program.cs                 # Application entry point and configuration
├── gRPC.csproj               # Project file (contains Protobuf references)
└── appsettings.json          # Configuration settings
```

### Key Differences from REST APIs:
- **No Controllers**: Services are defined in `.proto` files and implemented as classes
- **Binary Protocol**: Uses Protobuf instead of JSON
- **HTTP/2**: Requires HTTP/2 (not HTTP/1.1)
- **Contract-First**: The `.proto` file is the source of truth

## 4. Understanding the .proto File

The heart of gRPC is the `.proto` file. It defines the service contract using **Protocol Buffers** (Protobuf).

### Example: greet.proto

```protobuf
syntax = "proto3";                    // Protobuf version

option csharp_namespace = "gRPC";     // C# namespace for generated code

package greet;                        // Package name (optional)

// Service definition
service Greeter {
  // RPC method: takes HelloRequest, returns HelloReply
  rpc SayHello (HelloRequest) returns (HelloReply);
  rpc Ping (PingRequest) returns (PingReply);
}

// Request message
message HelloRequest {
  string name = 1;    // Field number 1 (used for binary encoding)
}

// Response message
message HelloReply {
  string message = 1;
}

message PingRequest {}

message PingReply {
  string message = 1;
}
```

### Key Concepts:
- **syntax**: Specifies Protobuf version (proto3 is current)
- **service**: Defines RPC methods (like REST endpoints)
- **message**: Defines data structures (like DTOs)
- **Field Numbers**: Used for binary serialization (never change these!)

## 5. The .csproj Configuration

The project file must reference `.proto` files:

```xml
<ItemGroup>
  <Protobuf Include="Protos\greet.proto" GrpcServices="Server" />
  <Protobuf Include="Protos\post.proto" GrpcServices="Server" />
</ItemGroup>
```

- **GrpcServices="Server"**: Generates server-side code
- **GrpcServices="Client"**: Generates client-side code
- **GrpcServices="Both"**: Generates both

## 6. Implementing a Service

Services inherit from the auto-generated base class:

```csharp
using Grpc.Core;

namespace gRPC.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(
            HelloRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
```

### Key Points:
- Inherit from `{ServiceName}Base` (auto-generated)
- Override RPC methods
- Use `ServerCallContext` for metadata, cancellation, etc.

## 7. Registering Services

In `Program.cs`, register your gRPC services:

```csharp
using gRPC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services to DI container
builder.Services.AddGrpc();

var app = builder.Build();

// Map gRPC services to endpoints
app.MapGrpcService<GreeterService>();
app.MapGrpcService<PostService>();

app.Run();
```

## 8. Running the Server

Start the gRPC server:

```powershell
dotnet run
# Or for hot-reload during development:
dotnet watch run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7021
```

> **Note**: The port is configured in `Properties/launchSettings.json`

## 9. Testing Your Service

Since gRPC uses HTTP/2 and binary data, you **cannot** test it in a browser. You need a gRPC client.

### Option 1: Create a Console Client (Recommended)

See the `gRPCClient` project in this repository for a full example.

### Option 2: Use grpcurl (Command Line)

```powershell
# Install grpcurl
# Then list services:
grpcurl -plaintext localhost:7021 list

# Call a method:
grpcurl -plaintext -d '{"name": "World"}' localhost:7021 greet.Greeter/SayHello
```

### Option 3: Use Postman

Postman now supports gRPC! Import your `.proto` files and test directly.

## 10. Common Issues

### SSL/TLS Errors
If you get SSL errors, trust the development certificate:
```powershell
dotnet dev-certs https --trust
```

### Port Already in Use
Change the port in `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:7021;http://localhost:5021"
```

## 🎓 Summary

In this module, you learned:
- ✅ What gRPC is and why it's faster than REST
- ✅ How to create a gRPC project
- ✅ Understanding `.proto` files and Protobuf
- ✅ Project structure and key components
- ✅ How to implement and register services
- ✅ How to run and test your gRPC server

## 📚 Next Steps

Continue to [Module 2: Protobuf & Messages](module_02.md) to learn about defining robust data contracts.
