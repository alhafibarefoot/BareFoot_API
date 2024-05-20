using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace MinAPI.EndPoints
{
    public static class StaticPost
    {
        public static RouteGroupBuilder MapStaticPost(this RouteGroupBuilder group)
        {
            //var builder = WebApplication.CreateBuilder();
            //var varMyEnv = builder.Configuration.GetValue<string>("myEnv");

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
            group
                .MapGet(
                    "/",
                    () =>
                    {
                        return Results.Ok(varNewslist);
                    }
                )
                .WithOpenApi(x => new OpenApiOperation(x)
                {
                    Summary = "إحضار جميع الأخبار",
                    Description =
                        "Returns information about all the available news from the Alhafi Blog."
                });

            group
                .MapGet(
                    "/{id}",
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
                .WithOpenApi();

            //Update specif Record
            group.MapPut(
                "/{id}",
                (NewsListStatic UpdateNewsListStatic, int id) =>
                {
                    var varNews = varNewslist.Find(c => c.Id == id);
                    if (varNews == null)
                        return Results.NotFound("Sorry this command doesn't exsists");

                    varNews.Title = UpdateNewsListStatic.Title;
                    varNews.Content = UpdateNewsListStatic.Content;

                    return Results.Ok(varNews);
                }
            );

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

            group.MapPost(
                "/",
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

                    NewNewsListStatic.Id =
                        varNewslist.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    varNewslist.Add(NewNewsListStatic);
                    return Results.Ok(varNewslist);
                }
            );

            /// <summary>
            ///Delete Specific News.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            group.MapDelete(
                "/{id}",
                (int id) =>
                {
                    var varNews = varNewslist.Find(c => c.Id == id);
                    if (varNews == null)
                        return Results.NotFound("Sorry this News doesn't exsists");
                    varNewslist.Remove(varNews);
                    return Results.Ok(varNews);
                }
            );

            //*************************Static Sample Hello*******************************************


            // group.MapGet("/", () => varMyEnv).WithTags("Demo");
            // group
            //     .MapGet("/Demo", (Data.Models.IDateTime dateTime) => dateTime.Now)
            //     .WithTags("Demo");
            // group
            //     .MapGet("/Demo2", ([FromServices] Data.Models.IDateTime dateTime) => dateTime.Now)
            //     .WithTags("Demo");

            //*****************Static Record End Points(Data Will not save after close )*****************
            return group;
        }
    }
}

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
