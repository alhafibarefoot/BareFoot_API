using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinAPI.Data;
using MinAPI.Data.Bindings;
using MinAPI.Data.Models;
using MinAPI.Validations;

namespace MinAPI.EndPoints
{
    public static class DBConext
    {
        public static RouteGroupBuilder MapDBConextPost(this RouteGroupBuilder group)
        {
            group
                .MapGet(
                    "/posts",
                    async (AppDbContext context) =>
                    {
                        var varPosts = await context.Posts.ToListAsync();
                        return Results.Ok(varPosts);
                    }
                )
                .WithDescription("return All posts news ")
                .WithSummary("احضار جميع الأخبار")
                .WithOpenApi()
                .CacheOutput(c => c.Expire(TimeSpan.FromDays(360)).Tag("Post_Get"));

            group
                .MapGet(
                    "/posts/{id}",
                    async (AppDbContext context, int id) =>
                    {
                        var varPost = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
                        if (varPost != null)
                        {
                            return Results.Ok(varPost);
                        }
                        return Results.NotFound();
                    }
                )
                .WithDescription("return Only One Post News")
                .WithSummary("احضار خبر واحد ")
                .WithOpenApi();

            group
                .MapPost(
                    "/posts",
                    async (
                        AppDbContext context,
                        Post poss,
                        IValidator<Post> validator,
                        IOutputCacheStore outputCacheRestore
                    ) =>
                    {
                        var validationResult = await validator.ValidateAsync(poss);

                        if (validationResult.IsValid)
                        {
                            await context.Posts.AddAsync(poss);
                            await context.SaveChangesAsync();
                            await outputCacheRestore.EvictByTagAsync("Post_Get", default);
                            return Results.Created($"/posts/{poss.Id}", poss);
                        }
                        return Results.ValidationProblem(
                            validationResult.ToDictionary(),
                            statusCode: (int)HttpStatusCode.UnprocessableEntity
                        );
                    }
                )
                .WithDescription("Insert New Post News")
                .WithSummary("ادخال خبر جديد ")
                .WithOpenApi();

            group
                .MapPost(
                    "/posts/v2",
                    async (
                        AppDbContext context,
                        [FromBody] Post poss,
                        IOutputCacheStore outputCacheRestore
                    ) =>
                    {
                        await context.Posts.AddAsync(poss);
                        await context.SaveChangesAsync();
                        await outputCacheRestore.EvictByTagAsync("Post_Get", default);
                        return Results.Created($"/posts/{poss.Id}", poss);
                    }
                )
                .AddEndpointFilter<ValidationFilter<Post>>()
                .WithDescription("Insert New Post News")
                .WithSummary("ادخال خبر جديد ")
                .WithOpenApi();

            group
                .MapPost(
                    "/posts/v3",
                    async (
                        AppDbContext context,
                        Post poss,
                        IOutputCacheStore outputCacheRestore
                    ) =>
                    {
                        await context.Posts.AddAsync(poss);
                        await context.SaveChangesAsync();
                        await outputCacheRestore.EvictByTagAsync("Post_Get", default);
                        return Results.Created($"/posts/{poss.Id}", poss);
                    }
                )
                .WithDescription("Insert New Post News")
                .WithSummary("ادخال خبر جديد ")
                .WithOpenApi();

            group
                .MapPut(
                    "/posts/{id}",
                    async (
                        AppDbContext context,
                        int id,
                        Post poss,
                        IOutputCacheStore outputCacheRestore
                    ) =>
                    {
                        var varPost = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
                        if (varPost != null)
                        {
                            varPost.Title = poss.Title;
                            varPost.Content = poss.Content;
                            varPost.postImage = poss.postImage;
                            await context.SaveChangesAsync();
                            await outputCacheRestore.EvictByTagAsync("Post_Get", default);
                            return Results.NoContent();
                        }
                        return Results.NotFound();
                    }
                )
                .WithDescription("Update Post News")
                .WithSummary("تعديل خبر  ")
                .WithOpenApi();

            group
                .MapDelete(
                    "/posts/{id}",
                    async (AppDbContext context, int id, IOutputCacheStore outputCacheRestore) =>
                    {
                        var varPost = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
                        if (varPost != null)
                        {
                            context.Posts.Remove(varPost);
                            await context.SaveChangesAsync();
                            await outputCacheRestore.EvictByTagAsync("Post_Get", default);
                            return Results.NoContent();
                        }
                        return Results.NotFound();
                    }
                )
                .WithDescription("Delete  Post ")
                .WithSummary("حذف خبر  ")
                .WithOpenApi();

            group
                .MapGet(
                    "/display",
                    ([AsParameters] Post post) =>
                    {
                        return $"Title: {post.Title}, Content: {post.Content}";
                    }
                )
                .WithDescription("View  Post ")
                .WithSummary("عرض خبر  ")
                .WithOpenApi();
            return group;
        }
    }
}
