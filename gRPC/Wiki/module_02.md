# Module 2: Protobuf & Messages

Unlike JSON in REST APIs, gRPC uses **Protocol Buffers (Protobuf)** - a language-neutral, platform-neutral mechanism for serializing structured data.

## 🎯 Why Protocol Buffers?

### Advantages over JSON:
- **Smaller Size**: 3-10x smaller payloads (binary vs text)
- **Faster**: 5-10x faster serialization/deserialization
- **Type Safety**: Compile-time type checking
- **Schema Evolution**: Backward and forward compatibility
- **Code Generation**: Automatic client/server code

### Trade-offs:
- ❌ Not human-readable (binary format)
- ❌ Requires `.proto` file definition
- ✅ But much better for machine-to-machine communication!

## 📊 Scalar Value Types

Protobuf supports strongly-typed data:

| Proto Type | C# Type | Default Value | Description |
|------------|---------|---------------|-------------|
| `double` | `double` | 0.0 | 64-bit floating point |
| `float` | `float` | 0.0 | 32-bit floating point |
| `int32` | `int` | 0 | Variable-length encoding |
| `int64` | `long` | 0 | Variable-length encoding |
| `uint32` | `uint` | 0 | Unsigned 32-bit |
| `uint64` | `ulong` | 0 | Unsigned 64-bit |
| `bool` | `bool` | false | Boolean |
| `string` | `string` | "" | UTF-8 or ASCII text |
| `bytes` | `ByteString` | empty | Arbitrary byte sequence |

### Example:
```protobuf
message Product {
  int32 id = 1;           // Product ID
  string name = 2;        // Product name
  double price = 3;       // Price (e.g., 19.99)
  bool in_stock = 4;      // Availability
  bytes thumbnail = 5;    // Image data
}
```

## 📦 Defining Messages

Messages are the data structures in Protobuf (like classes in C#).

### Basic Message:
```protobuf
message User {
  int32 id = 1;              // Field number 1
  string username = 2;       // Field number 2
  bool is_active = 3;        // Field number 3
}
```

### Field Numbers:
- **CRITICAL**: Field numbers are used for binary encoding
- **NEVER change** field numbers once deployed
- Numbers 1-15 use 1 byte, 16-2047 use 2 bytes
- Reserve frequently-used fields for 1-15

### Real Example from Our Project:

```protobuf
message PostModel {
  int32 id = 1;
  string title = 2;
  string content = 3;
  string post_image = 4;
  string created_on = 5;
}
```

## 🔄 Nested Messages

You can nest messages for complex structures:

```protobuf
message BlogPost {
  int32 id = 1;
  string title = 2;
  Author author = 3;        // Nested message
  repeated Comment comments = 4;  // List of comments
}

message Author {
  int32 id = 1;
  string name = 2;
}

message Comment {
  int32 id = 1;
  string text = 2;
}
```

## 📋 Repeated Fields (Lists/Arrays)

Use `repeated` for collections:

```protobuf
message ListPostsReply {
  repeated PostModel posts = 1;  // List of posts
}
```

In C#, this becomes:
```csharp
var reply = new ListPostsReply();
reply.Posts.AddRange(postsList);  // RepeatedField<PostModel>
```

## 🔧 Service Definition

Services define RPC methods (like REST endpoints):

```protobuf
service PostService {
  rpc CreatePost (CreatePostRequest) returns (PostModel);
  rpc ReadPost (ReadPostRequest) returns (PostModel);
  rpc ListPosts (ListPostsRequest) returns (ListPostsReply);
}
```

### RPC Method Types:

#### 1. Unary RPC (Request-Response)
```protobuf
rpc GetUser (UserRequest) returns (UserResponse);
```
Like a normal REST API call.

#### 2. Server Streaming
```protobuf
rpc ListUsers (ListRequest) returns (stream UserResponse);
```
Server sends multiple responses.

#### 3. Client Streaming
```protobuf
rpc UploadData (stream DataChunk) returns (UploadResponse);
```
Client sends multiple requests.

#### 4. Bi-directional Streaming
```protobuf
rpc Chat (stream Message) returns (stream Message);
```
Both sides stream data.

## 🏗️ Complete Example: Post Service

Here's our complete `post.proto` file:

```protobuf
syntax = "proto3";

option csharp_namespace = "gRPC";

package post;

service PostService {
  rpc CreatePost (CreatePostRequest) returns (PostModel);
  rpc ReadPost (ReadPostRequest) returns (PostModel);
  rpc ListPosts (ListPostsRequest) returns (ListPostsReply);
}

message PostModel {
  int32 id = 1;
  string title = 2;
  string content = 3;
  string post_image = 4;
  string created_on = 5;
}

message CreatePostRequest {
  string title = 1;
  string content = 2;
}

message ReadPostRequest {
  int32 id = 1;
}

message ListPostsRequest {}

message ListPostsReply {
  repeated PostModel posts = 1;
}
```

## ⚙️ Compilation & Code Generation

When you build the project, the .NET tooling automatically generates C# code from `.proto` files.

### What Gets Generated:

1. **Message Classes**: C# classes for each message
   ```csharp
   public class PostModel { ... }
   public class CreatePostRequest { ... }
   ```

2. **Service Base Class**: Abstract class to implement
   ```csharp
   public abstract class PostServiceBase { ... }
   ```

3. **Client Class**: For calling the service
   ```csharp
   public class PostServiceClient { ... }
   ```

### Build Process:

```powershell
dotnet build
```

Generated files are in: `obj/Debug/net9.0/Protos/`

## 📝 Naming Conventions

### Proto Files:
- Use `snake_case` for fields: `created_on`, `post_image`
- Use `PascalCase` for messages: `PostModel`, `CreatePostRequest`
- Use `PascalCase` for services: `PostService`

### Generated C# Code:
- Fields become `PascalCase` properties: `CreatedOn`, `PostImage`
- Messages stay `PascalCase`: `PostModel`

## 🎯 Best Practices

1. **Never Change Field Numbers**: Once deployed, field numbers are permanent
2. **Reserve Deleted Fields**: If you remove a field, reserve its number
   ```protobuf
   reserved 4, 5;  // Don't reuse these numbers
   ```
3. **Use Descriptive Names**: `CreatePostRequest` not just `Request`
4. **Group Related Messages**: Keep request/response pairs together
5. **Use Comments**: Document your proto files
   ```protobuf
   // Creates a new blog post
   rpc CreatePost (CreatePostRequest) returns (PostModel);
   ```

## 🔄 Schema Evolution

Protobuf supports backward/forward compatibility:

### Adding Fields (Safe):
```protobuf
message User {
  int32 id = 1;
  string name = 2;
  string email = 3;  // ✅ New field - old clients ignore it
}
```

### Removing Fields (Use Reserved):
```protobuf
message User {
  int32 id = 1;
  string name = 2;
  reserved 3;        // ✅ Email removed, number reserved
  reserved "email";  // ✅ Name also reserved
}
```

### Changing Types (Dangerous):
❌ **Don't change field types** - it breaks compatibility!

## 🎓 Summary

In this module, you learned:
- ✅ Why Protobuf is faster and more efficient than JSON
- ✅ Protobuf scalar types and their C# equivalents
- ✅ How to define messages and services
- ✅ Using repeated fields for collections
- ✅ Different RPC method types (unary, streaming)
- ✅ Code generation and naming conventions
- ✅ Best practices for schema evolution

## 📚 Next Steps

Continue to [Module 3: Database Integration](module_03.md) to learn how to connect Entity Framework Core with gRPC services.
