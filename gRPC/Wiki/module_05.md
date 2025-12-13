# Module 5: Streaming & Data

gRPC shines with streaming.

## 1. Server Streaming
The server sends multiple responses to a single request. Perfect for large datasets or real-time updates.

```protobuf
rpc GetStream(Request) returns (stream Response);
```

Implementation:
```csharp
public override async Task GetStream(Request req, IServerStreamWriter<Response> stream, ServerCallContext context)
{
    foreach(var item in data) {
         await stream.WriteAsync(item);
    }
}
```

## 2. Client Streaming
The client sends a stream of messages. Good for large file uploads.

## 3. Bi-directional Streaming
Both sides stream. Chat apps, gaming, etc.
