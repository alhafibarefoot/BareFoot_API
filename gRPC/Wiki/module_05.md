# Module 5: Streaming & High-Performance Data

gRPC shines with streaming capabilities - one of its most powerful features that sets it apart from traditional REST APIs.

## 🎯 Why Streaming?

### Use Cases:
- **Real-time Updates**: Stock prices, live scores, notifications
- **Large Datasets**: Paginating through millions of records
- **File Transfers**: Uploading/downloading large files in chunks
- **Chat Applications**: Bi-directional messaging
- **IoT Data**: Continuous sensor data streams

### Benefits:
- ✅ **Efficient**: Send data as it becomes available
- ✅ **Low Latency**: No need to wait for complete dataset
- ✅ **Memory Efficient**: Process data in chunks
- ✅ **Real-time**: Perfect for live updates

## 📡 Types of Streaming

### 1. Unary RPC (No Streaming)
Standard request-response (what we've been using):
```protobuf
rpc GetPost (PostRequest) returns (PostResponse);
```

### 2. Server Streaming
Client sends one request, server sends multiple responses:
```protobuf
rpc StreamPosts (StreamRequest) returns (stream PostModel);
```

### 3. Client Streaming
Client sends multiple requests, server sends one response:
```protobuf
rpc UploadPosts (stream PostModel) returns (UploadResponse);
```

### 4. Bidirectional Streaming
Both client and server send streams:
```protobuf
rpc Chat (stream Message) returns (stream Message);
```

## 🔽 Server Streaming

The server sends multiple responses to a single client request.

### Proto Definition:

```protobuf
service PostService {
  rpc StreamPosts (StreamPostsRequest) returns (stream PostModel);
}

message StreamPostsRequest {
  int32 batch_size = 1;
}
```

### Server Implementation:

```csharp
public override async Task StreamPosts(
    StreamPostsRequest request,
    IServerStreamWriter<PostModel> responseStream,
    ServerCallContext context)
{
    var posts = await _context.Posts.ToListAsync();

    foreach (var post in posts)
    {
        // Check if client cancelled
        if (context.CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Client cancelled stream");
            break;
        }

        // Send each post to the stream
        await responseStream.WriteAsync(new PostModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedOn = post.CreatedOn.ToString("o")
        });

        // Optional: Add delay to simulate real-time updates
        await Task.Delay(100);
    }
}
```

### Client Usage:

```csharp
var call = client.StreamPosts(new StreamPostsRequest { BatchSize = 10 });

await foreach (var post in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"Received: [{post.Id}] {post.Title}");
}
```

### Real-World Example: Live Feed

```csharp
public override async Task LivePostFeed(
    Empty request,
    IServerStreamWriter<PostModel> responseStream,
    ServerCallContext context)
{
    while (!context.CancellationToken.IsCancellationRequested)
    {
        // Get latest posts
        var latestPost = await _context.Posts
            .OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync();

        if (latestPost != null)
        {
            await responseStream.WriteAsync(_mapper.Map<PostModel>(latestPost));
        }

        // Wait before checking again
        await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);
    }
}
```

## 🔼 Client Streaming

The client sends multiple requests, server sends one response.

### Proto Definition:

```protobuf
service PostService {
  rpc BulkCreatePosts (stream CreatePostRequest) returns (BulkCreateResponse);
}

message BulkCreateResponse {
  int32 created_count = 1;
  repeated int32 post_ids = 2;
}
```

### Server Implementation:

```csharp
public override async Task<BulkCreateResponse> BulkCreatePosts(
    IAsyncStreamReader<CreatePostRequest> requestStream,
    ServerCallContext context)
{
    var createdIds = new List<int>();

    // Read all incoming requests
    await foreach (var request in requestStream.ReadAllAsync())
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            CreatedOn = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        createdIds.Add(post.Id);
    }

    return new BulkCreateResponse
    {
        CreatedCount = createdIds.Count,
        PostIds = { createdIds }
    };
}
```

### Client Usage:

```csharp
using var call = client.BulkCreatePosts();

// Send multiple posts
for (int i = 0; i < 100; i++)
{
    await call.RequestStream.WriteAsync(new CreatePostRequest
    {
        Title = $"Post {i}",
        Content = $"Content for post {i}"
    });
}

// Signal completion
await call.RequestStream.CompleteAsync();

// Get response
var response = await call;
Console.WriteLine($"Created {response.CreatedCount} posts");
```

## 🔄 Bidirectional Streaming

Both client and server send streams independently.

### Proto Definition:

```protobuf
service ChatService {
  rpc Chat (stream ChatMessage) returns (stream ChatMessage);
}

message ChatMessage {
  string user = 1;
  string message = 2;
  string timestamp = 3;
}
```

### Server Implementation:

```csharp
public override async Task Chat(
    IAsyncStreamReader<ChatMessage> requestStream,
    IServerStreamWriter<ChatMessage> responseStream,
    ServerCallContext context)
{
    // Read messages from client
    await foreach (var message in requestStream.ReadAllAsync())
    {
        _logger.LogInformation($"Received: {message.User}: {message.Message}");

        // Echo back or broadcast to other clients
        await responseStream.WriteAsync(new ChatMessage
        {
            User = "Server",
            Message = $"Echo: {message.Message}",
            Timestamp = DateTime.UtcNow.ToString("o")
        });
    }
}
```

### Client Usage:

```csharp
using var call = client.Chat();

// Start reading responses in background
var readTask = Task.Run(async () =>
{
    await foreach (var response in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"{response.User}: {response.Message}");
    }
});

// Send messages
while (true)
{
    var input = Console.ReadLine();
    if (input == "exit") break;

    await call.RequestStream.WriteAsync(new ChatMessage
    {
        User = "Client",
        Message = input,
        Timestamp = DateTime.UtcNow.ToString("o")
    });
}

await call.RequestStream.CompleteAsync();
await readTask;
```

## ⚡ Performance Considerations

### Backpressure Handling:

```csharp
public override async Task StreamLargeDataset(
    DataRequest request,
    IServerStreamWriter<DataChunk> responseStream,
    ServerCallContext context)
{
    const int batchSize = 100;
    var offset = 0;

    while (true)
    {
        var batch = await _context.Data
            .Skip(offset)
            .Take(batchSize)
            .ToListAsync();

        if (!batch.Any()) break;

        foreach (var item in batch)
        {
            await responseStream.WriteAsync(_mapper.Map<DataChunk>(item));
        }

        offset += batchSize;

        // Allow client to process
        await Task.Delay(10);
    }
}
```

### Cancellation Support:

```csharp
public override async Task LongRunningStream(
    Empty request,
    IServerStreamWriter<StatusUpdate> responseStream,
    ServerCallContext context)
{
    try
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new StatusUpdate
            {
                Message = "Still running...",
                Timestamp = DateTime.UtcNow.ToString("o")
            });

            await Task.Delay(1000, context.CancellationToken);
        }
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("Stream cancelled by client");
    }
}
```

## 🎯 Best Practices

### ✅ Do:
- Always check `context.CancellationToken` in loops
- Use `IAsyncEnumerable` with `await foreach`
- Handle client disconnections gracefully
- Implement backpressure for large streams
- Log stream start/end for debugging

### ❌ Don't:
- Don't send entire dataset at once - defeats the purpose
- Don't forget to call `CompleteAsync()` on client streams
- Don't ignore cancellation tokens
- Don't block the stream with synchronous operations

## 📊 Streaming vs Unary Comparison

| Feature | Unary RPC | Streaming |
|---------|-----------|-----------|
| **Latency** | Wait for complete response | Immediate first result |
| **Memory** | Load entire dataset | Process in chunks |
| **Use Case** | Small, complete data | Large/continuous data |
| **Complexity** | Simple | More complex |
| **Real-time** | No | Yes |

## 🎓 Summary

In this module, you learned:
- ✅ Four types of RPC methods (unary, server, client, bidirectional streaming)
- ✅ How to implement server streaming for real-time updates
- ✅ Client streaming for bulk operations
- ✅ Bidirectional streaming for chat-like applications
- ✅ Performance considerations and best practices
- ✅ Cancellation and backpressure handling

## 📚 Next Steps

Continue to [Module 6: Robustness](module_06.md) to learn about validation and error handling with interceptors.
