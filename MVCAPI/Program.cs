using MVCAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Services (Controllers, Swagger, DB, etc.)
builder.RegisterServices();

var app = builder.Build();

// Configure Middleware (SwaggerUI, Auth, MapControllers)
app.RegisterMiddlewares();

app.Run();
