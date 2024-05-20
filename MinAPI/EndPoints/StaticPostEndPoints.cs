using Microsoft.OpenApi.Models;

namespace MinAPI.EndPoints
{
    public static class StaticPost
    {
        public static RouteGroupBuilder MapStaticPost(this RouteGroupBuilder group)
        {
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

            return group;
        }
    }
}
