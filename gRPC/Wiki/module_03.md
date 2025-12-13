# Module 3: Database Integration

Integrating EF Core with gRPC is very similar to REST, but we inject the `DbContext` into the **Service** class.

## 1. DbContext
Setup `AppDbContext` as usual.

## 2. Dependency Injection
In `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(op => ...);
```

## 3. Usage in Service
```csharp
public class MyService : MyServiceProtos.MyServiceBase
{
    private readonly AppDbContext _db;
    public MyService(AppDbContext db) => _db = db;

    public override async Task<Reply> GetData(Request request, ServerCallContext context)
    {
        var data = await _db.Items.FindAsync(request.Id);
        // Map to Protobuf Message...
    }
}
```
