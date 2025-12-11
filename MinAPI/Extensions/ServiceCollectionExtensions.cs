using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinAPI.Data;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;
using MinAPI.Data.Repositories;
using MinAPI.Services;
using MinAPI.Services.Interfaces;
using MinAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MinAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<AppDbContext>(x =>
                x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IPostRepo, PostRepo>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Identity Configuration
            builder.Services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>();

            // JWT Configuration
            var jwtSettingsLine = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettingsLine["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Simplified for dev
                    ValidateAudience = false, // Simplified for dev
                    //ValidIssuer = jwtSettingsLine["Issuer"],
                    //ValidAudience = jwtSettingsLine["Audience"]
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            //builder.Services.AddValidatorsFromAssemblyContaining(typeof(PostValidator));
            //builder.Services.AddScoped<IValidator<Post>, PostValidator>();
            //builder.Services.AddScoped<IValidator<PostNewOrUpdatedDto>, PostNewOrUpdatedDtoValidator>();


            builder.Services.AddSingleton<IDateTime, SystemDateTime>();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>(
                ServiceLifetime.Singleton
            );
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(60)));
                options.AddPolicy("PostCache", builder => builder.Expire(TimeSpan.FromDays(360)).Tag("Post_Get"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "MyAllowedOrigins",
                    policy =>
                    {
                        var origins = builder
                            .Configuration.GetSection("CorsSettings:AllowedOrigins")
                            .Get<string[]>();

                        if (origins != null && origins.Length > 0)
                        {
                            policy
                                .WithOrigins(origins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        }
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

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // using System.Reflection;
                var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                // integrate xml comments
                c.IncludeXmlComments(filePath);
            });
        }
    }
}
