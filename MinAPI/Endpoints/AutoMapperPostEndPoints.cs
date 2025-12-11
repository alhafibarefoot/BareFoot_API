using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;
using MinAPI.Services.Interfaces;
using MinAPI.Validations;
using static MinAPI.Data.DTOs.PostDTOs;

namespace MinAPI.Endpoints
{
    public static class AutoMapperPostEndpoints
    {
        public static RouteGroupBuilder MapAutoMapperPost(this RouteGroupBuilder group)
        {
            //Creating Variables of Lists
            group
                .MapGet(
                    "/automapper/posts",
                    async (
                        IPostService service,
                        [AsParameters] MinAPI.Data.DTOs.PostQueryParameters parameters
                    ) =>
                    {
                        var posts = await service.GetAllPostsAsync(parameters);
                        return Results.Ok(posts);
                    }
                )
                .WithDescription("return All posts news ")
                .WithSummary("احضار جميع الأخبار")
                .WithOpenApi();

            group
                .MapGet(
                    "/automapper/posts/{id}",
                    async (IPostService service, int id) =>
                    {
                        var post = await service.GetPostByIdAsync(id);
                        if (post != null)
                        {
                            return Results.Ok(post);
                        }
                        return Results.NotFound();
                    }
                )
                .WithDescription("return Only One Post News")
                .WithSummary("احضار خبر واحد ")
                .WithOpenApi();

            group
                .MapPost(
                    "/automapper/posts",
                    async (
                        IPostService service,
                        [FromForm] PostNewOrUpdatedDto postCreateDto
                    ) =>
                    {
                        var createdPost = await service.CreatePostAsync(postCreateDto);
                        return Results.Created($"/automapper/posts/{createdPost.Id}", createdPost);
                    }
                )
                .DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<PostNewOrUpdatedDto>>()
                .WithDescription("Insert New Post News")
                .WithSummary("ادخال خبر جديد ")
                .WithOpenApi();

            group
                .MapPut(
                    "/automapper/posts/{id}",
                    async (
                        IPostService service,
                        int id,
                        [FromForm] PostNewOrUpdatedDto postUpdateDto
                    ) =>
                    {
                        var result = await service.UpdatePostAsync(id, postUpdateDto);
                        if (!result)
                        {
                            return Results.NotFound();
                        }
                        return Results.NoContent();
                    }
                )
                .WithDescription("Update Post News")
                .WithSummary("تعديل خبر  ")
                .WithOpenApi();

            group
                .MapDelete(
                    "/automapper/posts/{id}",
                    async (IPostService service, int id) =>
                    {
                        var result = await service.DeletePostAsync(id);
                        if (!result)
                        {
                            return Results.NotFound();
                        }
                        return Results.NoContent();
                    }
                )
                .WithDescription("Delete  Post ")
                .WithSummary("حذف خبر  ")
                .WithOpenApi();
            return group;
        }
    }
}
