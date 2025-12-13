# Module 6: Robustness

## Validation
You can use FluentValidation. Since there is no "ModelState", you often use **Interceptors** to run validation before the method executes.

## Global Exception Handling
Use an **Interceptor** (Middleware) to catch exceptions and return valid `RpcException` with standard Status Codes (NotFound, InvalidArgument, etc.).

```csharp
public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler(...)
    {
        try { return await continuation(request, context); }
        catch (Exception ex) { throw new RpcException(new Status(StatusCode.Internal, ex.Message)); }
    }
}
```
