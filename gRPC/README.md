# 🦅 gRPC API - High-Performance RPC Framework

A modern, high-performance gRPC service implementation in .NET 9 demonstrating contract-based API development using Protocol Buffers (Protobuf).

## 🌟 Overview

This project showcases how to build blazing-fast, type-safe APIs using gRPC - a modern Remote Procedure Call (RPC) framework that uses HTTP/2 and binary serialization for superior performance compared to traditional REST APIs.

### Why gRPC?

- **⚡ Performance**: Up to 7x faster than REST APIs using HTTP/2 and binary Protobuf serialization
- **🔒 Type Safety**: Strongly-typed contracts defined in `.proto` files
- **🌐 Cross-Platform**: Works seamlessly across different languages and platforms
- **📡 Streaming**: Built-in support for server, client, and bi-directional streaming
- **🎯 Perfect for Microservices**: Ideal for internal service-to-service communication

## 🏗️ Project Structure

```
gRPC/
├── Protos/                    # Protocol Buffer definitions
│   ├── greet.proto           # Greeter service contract
│   └── post.proto            # Post CRUD service contract
├── Services/                  # Service implementations
│   ├── GreeterService.cs     # Hello world service
│   └── PostService.cs        # Post CRUD operations
├── Data/                      # Database layer
│   ├── AppDbContext.cs       # EF Core context
│   └── Models/               # Entity models
├── gRPCClient/               # Console client application
│   ├── Program.cs            # Interactive client demo
│   └── gRPCClient.csproj     # Client project file
├── Wiki/                      # Comprehensive documentation
│   ├── Home.md               # Wiki home page
│   └── module_*.md           # Learning modules
├── Program.cs                 # Server configuration
└── gRPC.csproj               # Server project file
```

## 🚀 Features

### Implemented Services

#### 1. Greeter Service
- **SayHello**: Simple greeting RPC
- **Ping**: Health check endpoint

#### 2. Post Service (Full CRUD Operations)
- **CreatePost**: Create new blog posts
- **ReadPost**: Retrieve post by ID
- **UpdatePost**: Update existing post (title and content)
- **DeletePost**: Delete post by ID
- **ListPosts**: Get all posts

### Technical Features

- ✅ Protocol Buffers (proto3) contracts
- ✅ Entity Framework Core integration with SQLite
- ✅ Dependency Injection
- ✅ Structured logging
- ✅ Error handling with RpcException
- ✅ Interactive console client
- ✅ Database migrations

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 / VS Code / Rider (optional)
- Basic understanding of C# and async/await

## 🛠️ Installation & Setup

### 1. Clone the Repository

```powershell
git clone <repository-url>
cd BareFoot_API/gRPC
```

### 2. Restore Dependencies

```powershell
dotnet restore
```

### 3. Setup Database

The project uses SQLite with Entity Framework Core. Run migrations:

```powershell
dotnet ef database update
```

This creates `gRPC.db` in the project root.

### 4. Run the Server

```powershell
dotnet run
# Or for hot-reload during development:
dotnet watch run
```

The server will start on `https://localhost:7021` (verify in `Properties/launchSettings.json`).

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7021
```

## 🧪 Testing the API

### Option 1: Using the Included gRPC Client (Recommended)

The project includes a fully-featured interactive console client.

#### Run the Client

```powershell
cd gRPCClient
dotnet run
```

#### Client Features

The client provides an interactive menu:

```
=== gRPC Client - Post Manager ===

✓ Connected: Hello Barefoot User

--- Menu ---
1. Create New Post
2. Read Post by ID
3. Update Post
4. Delete Post
5. List All Posts
6. Exit

Select option:
```

**Example Usage:**

1. **Create a Post**: Select option 1, enter title and content
2. **Read a Post**: Select option 2, enter the post ID
3. **Update a Post**: Select option 3, enter post ID and new data
4. **Delete a Post**: Select option 4, enter post ID (with confirmation)
5. **List All Posts**: Select option 5 to see all posts

### Option 2: Create Your Own Client

```powershell
# Create new console app
dotnet new console -n MyGrpcClient
cd MyGrpcClient

# Add required packages
dotnet add package Grpc.Net.Client
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools
```

Edit `MyGrpcClient.csproj` to reference the proto files:

```xml
<ItemGroup>
  <Protobuf Include="..\Protos\greet.proto" GrpcServices="Client" />
  <Protobuf Include="..\Protos\post.proto" GrpcServices="Client" />
</ItemGroup>
```

Example client code:

```csharp
using Grpc.Net.Client;
using gRPC;

using var channel = GrpcChannel.ForAddress("https://localhost:7021");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "World" });
Console.WriteLine(reply.Message);
```

### Option 3: Using gRPCurl (Command Line)

Install [grpcurl](https://github.com/fullstorydev/grpcurl):

```powershell
# List available services
grpcurl -plaintext localhost:7021 list

# Call SayHello
grpcurl -plaintext -d '{"name": "World"}' localhost:7021 greet.Greeter/SayHello

# Create a post
grpcurl -plaintext -d '{"title": "My Post", "content": "Hello gRPC!"}' localhost:7021 post.PostService/CreatePost
```

### Option 4: Using Postman

Postman supports gRPC!

1. Create a new gRPC request
2. Enter server URL: `localhost:7021`
3. Import the `.proto` files from the `Protos/` folder
4. Select the service method and send requests

## 📦 Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Runtime framework |
| Grpc.AspNetCore | 2.64.0 | gRPC server implementation |
| Entity Framework Core | 9.0.0 | ORM for database access |
| SQLite | 9.0.0 | Lightweight database |
| Protocol Buffers | proto3 | Service contract definition |
| Grpc.Net.Client | 2.71.0 | gRPC client library |

## 📚 Protocol Buffer Contracts

### greet.proto

```protobuf
service Greeter {
  rpc SayHello (HelloRequest) returns (HelloReply);
  rpc Ping (PingRequest) returns (PingReply);
}
```

### post.proto

```protobuf
service PostService {
  rpc CreatePost (CreatePostRequest) returns (PostModel);
  rpc ReadPost (ReadPostRequest) returns (PostModel);
  rpc UpdatePost (UpdatePostRequest) returns (PostModel);
  rpc DeletePost (DeletePostRequest) returns (DeletePostReply);
  rpc ListPosts (ListPostsRequest) returns (ListPostsReply);
}
```

Full proto definitions are in the `Protos/` directory.

## 🎓 Learning Resources

This project includes comprehensive Wiki documentation:

| Module | Topic | Description |
|--------|-------|-------------|
| [Module 1](Wiki/module_01.md) | Fundamentals | Project setup, structure, and `.proto` basics |
| [Module 2](Wiki/module_02.md) | Protobuf & Messages | Defining robust data contracts |
| [Module 3](Wiki/module_03.md) | Database Integration | EF Core with gRPC services |
| [Module 4](Wiki/module_04.md) | Professional Patterns | DTOs, AutoMapper, and mapping |
| [Module 5](Wiki/module_05.md) | Streaming | Server, client, and bi-directional streaming |
| [Module 6](Wiki/module_06.md) | Robustness | Validation and error handling with interceptors |
| [Module 7](Wiki/module_07.md) | Security | JWT authentication and authorization |

**Start here**: [📖 Wiki Home](Wiki/Home.md)

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=gRPC.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Kestrel Configuration

The server is configured to use HTTP/2 (required for gRPC) in `Program.cs`:

```csharp
builder.Services.AddGrpc();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<PostService>();
```

## 🐛 Troubleshooting

### Common Issues

**Issue**: "The SSL connection could not be established"
- **Solution**: Ensure you trust the development certificate:
  ```powershell
  dotnet dev-certs https --trust
  ```

**Issue**: "No connection could be made because the target machine actively refused it"
- **Solution**: Make sure the server is running on the correct port (check `launchSettings.json`)

**Issue**: "Unimplemented RPC"
- **Solution**: Verify the service is registered in `Program.cs` with `app.MapGrpcService<YourService>()`

**Issue**: Database errors
- **Solution**: Run migrations:
  ```powershell
  dotnet ef database update
  ```

## 🔄 Development Workflow

1. **Define Contract**: Edit `.proto` files in `Protos/`
2. **Build Project**: The build process auto-generates C# classes from `.proto` files
3. **Implement Service**: Create/update service classes in `Services/`
4. **Register Service**: Add to `Program.cs` with `app.MapGrpcService<T>()`
5. **Test**: Use the client or testing tools

## 📈 Performance Comparison

| Metric | REST (JSON) | gRPC (Protobuf) | Improvement |
|--------|-------------|-----------------|-------------|
| Payload Size | ~500 bytes | ~70 bytes | **7x smaller** |
| Serialization | Text-based | Binary | **3-5x faster** |
| HTTP Version | HTTP/1.1 | HTTP/2 | Multiplexing, header compression |
| Type Safety | Runtime | Compile-time | Fewer bugs |

## 🤝 Contributing

This is an educational project. Feel free to:
- Add new services
- Implement streaming examples
- Add interceptors for logging/auth
- Improve error handling
- Add unit tests

## 📄 License

This project is part of the Barefoot API educational course.

## 🔗 Related Projects

- [MinAPI](../MinAPI/README.md) - Minimal API implementation
- [MVCAPI](../MVCAPI/README.md) - MVC-style API implementation

## 📞 Support

For questions or issues:
- Check the [Wiki documentation](Wiki/Home.md)
- Review the [main repository README](../README.md)
- Examine the working client in `gRPCClient/`

---

**Happy gRPC coding! 🦅**
