using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinAPI.Data;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;

namespace MinAPI.Points.ServicePoints
{
    public static class ConfigurationServices
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<AppDbContext>(x =>
                x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IPostRepo, PostRepo>();

            //builder.Services.AddValidatorsFromAssemblyContaining(typeof(PostValidator));
            //builder.Services.AddScoped<IValidator<Post>, PostValidator>();
            //builder.Services.AddScoped<IValidator<PostNewOrUpdatedDto>, PostNewOrUpdatedDtoValidator>();


            builder.Services.AddSingleton<IDateTime, SystemDateTime>();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>(
                ServiceLifetime.Singleton
            );
            builder.Services.AddOutputCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "MyAllowedOrigins",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                "https://localhost/*",
                                "http://localhost/*",
                                "http://127.0.0.1/*",
                                "https://www.alhafi.org"
                            )
                            .AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                );
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Barefoot API",
                        Version = "v1",
                        Contact = new()
                        {
                            Name = "Alhafi.BareFoot",
                            Email = "alhafi@hotmail.com",
                            Url = new Uri("https://www.alhafi.org/")
                        },
                        Description =
                            " BareFoot Minimal API Build in <b>dotnet new webapi -minimal</b>  Hosted at github  <a href='https://github.com/alhafibarefoot/BareFoot_API'>here</a>",
                        License = new Microsoft.OpenApi.Models.OpenApiLicense(),
                        TermsOfService = new("https://www.alhafi.org/")
                    }
                );

                // using System.Reflection;
                var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                // integrate xml comments
                c.IncludeXmlComments(filePath);
            });
        }
    }
}
