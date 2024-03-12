using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Creating Variables of Lists
var varNewslist = new List<NewsListStatic>
{
    new NewsListStatic
    {
        Id = 1,
        Title = "F1 News",
        Content = "Christian Horner Surprised By Team Strategy At Bahrain GP ."
    },
    new NewsListStatic
    {
        Id = 2,
        Title = "Haley will win",
        Content =
            "Former South Carolina Gov. Nikki Haley will win the Republican presidential primary in Washington, DC"
    },
};

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
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

//*************************Static Hello End Point*******************************************

app.MapGet("/hello", () => "[GET] Hello World!").WithTags("Hello");
app.MapPost("/hello", () => "[POST] Hello World!").WithTags("Hello");
app.MapPut("/hello", () => "[PUT] Hello World!").WithTags("Hello");
app.MapDelete("/hello", () => "[DELETE] Hello World!").WithTags("Hello");

//*****************Static Record End Points(Data Will not save after close )*****************

app.MapGet(
        "/staticNews",
        () =>
        {
            return Results.Ok(varNewslist);
        }
    )
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "إحضار جميع الأخبار",
        Description = "Returns information about all the available news from the Alhafi Blog.",
        Tags = new List<OpenApiTag> { new() { Name = "Static News" } }
    });

app.MapGet(
        "/staticNews/{id}",
        (int id) =>
        {
            var varNews = varNewslist.Find(c => c.Id == id);
            if (varNews == null)
                return Results.NotFound("Sorry this News doesn't exsists");
            return Results.Ok(varNews);
        }
    )
    .WithDescription("return one news ")
    .WithSummary("إحضار خبر واحد بناء على قيمة رقم السجل")
    .WithName("GetStaticNewsbyID")
    .WithTags("Static News")
    .WithOpenApi();

//Update specif Record
app.MapPut(
        "/staticNews/{id}",
        (NewsListStatic UpdateNewsListStatic, int id) =>
        {
            var varNews = varNewslist.Find(c => c.Id == id);
            if (varNews == null)
                return Results.NotFound("Sorry this command doesn't exsists");

            varNews.Title = UpdateNewsListStatic.Title;
            varNews.Content = UpdateNewsListStatic.Content;

            return Results.Ok(varNews);
        }
    )
    .WithTags("Static News");

/// <summary>
/// Creates a News.
/// </summary>
/// <param name="NewNewsListStatic"></param>
/// <returns>A newly created News Item</returns>
/// <remarks>
/// Sample request:
///
///     POST /Todo
///     {
///        "id": 1,
///        "Title": "F1 News",
///        "Content": "Fi car go Fast"
///     }
///
/// </remarks>
/// <response code="201">Returns the newly created item</response>
/// <response code="400">If the item is null</response>

app.MapPost(
        "/staticNews",
        (NewsListStatic NewNewsListStatic) =>
        {
            if (NewNewsListStatic.Id != 0 || string.IsNullOrEmpty(NewNewsListStatic.Title))
            {
                return Results.BadRequest("Invalid Id or HowTo filling");
            }
            if (
                varNewslist.FirstOrDefault(c =>
                    c.Title.ToLower() == NewNewsListStatic.Title.ToLower()
                ) != null
            )
            {
                return Results.BadRequest("HowTo Exsists");
            }

            NewNewsListStatic.Id = varNewslist.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
            varNewslist.Add(NewNewsListStatic);
            return Results.Ok(varNewslist);
        }
    )
    .WithTags("Static News");

/// <summary>
///Delete Specific News.
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
app.MapDelete(
        "/staticNews/{id}",
        (int id) =>
        {
            var varNews = varNewslist.Find(c => c.Id == id);
            if (varNews == null)
                return Results.NotFound("Sorry this News doesn't exsists");
            varNewslist.Remove(varNews);
            return Results.Ok(varNews);
        }
    )
    .WithTags("Static News");

//*******************************************************************************************

app.Run();

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
