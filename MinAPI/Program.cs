using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinAPI.Data;
using MinAPI.Data.Bindings;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;
using MinAPI.EndPoints;
using MinAPI.Validations;
using static MinAPI.Data.DTOs.PostDTOs;

//******************************************************* Var Zone *****************************************************

var builder = WebApplication.CreateBuilder(args);

var varMyEnv = builder.Configuration.GetValue<string>("myEnv");

//******************************************************* Ending Var Zone *****************************************************

//******************************************************* Services  Zone *****************************************************

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
builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
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

//******************************************************* Ending Service Zone *****************************************************

var app = builder.Build();

//******************************************************* Middle Points Zone( HTTP request pipeline) ***************************************************

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BareFoot API V1");
        options.RoutePrefix = "swagger";
        // options.RoutePrefix = string.Empty;  //direct root
        options.InjectStylesheet("/css/swagger.css");
    });
}
if (app.Environment.IsStaging())
{
    // your code here
}
if (app.Environment.IsProduction())
{
    // your code here
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("MyAllowedOrigins");

app.UseOutputCache();

//******************************************************* Ending Middle Points  *****************************************************


//******************************************************* End Points Zone *****************************************************

//*************************Static Sample Hello*******************************************


app.MapGet("/", () => varMyEnv).WithTags("Demo");
app.MapGet("/Demo", (IDateTime dateTime) => dateTime.Now).WithTags("Demo");
app.MapGet("/Demo2", ([FromServices] IDateTime dateTime) => dateTime.Now).WithTags("Demo");

app.MapGroup("/hello").MapHello().WithTags("Hello");

//*****************Static Record End Points(Data Will not save after close )*****************

app.MapGroup("/staticpost").MapStaticPost().WithTags("StaticPostNews");

//*************************Dynamic Data -Repository- Data Access(DB Context) EndPoint**************************


app.MapGroup("/dbcontext").MapDBConextPost().WithTags("DBContextPostNews");

//*************************Daynamic Data Access Automapper End Point*******************************************

app.MapGroup("/automapper").MapAutoMapperPost().WithTags("AutoMapperPostNews");

//******************************************************* Ending End Point *****************************************************

app.Run();

//*************************End of Programs.cs***********************************************

record NewsListStatic
{
    public int Id { get; set; }

    /// <summary>
    /// Title is Any headline news
    /// </summary>
    /// <example>Formula One World Championship</example>
    public string? Title { get; set; }

    /// <summary>
    /// Contenty contain details of the news
    /// </summary>
    /// <example>
    ///  2024 FIA Formula One World Championship is a motor racing championship
    ///  for Formula One cars and is the 75th running of the Formula One World Championship.
    /// </example>
    public string? Content { get; set; }
}
