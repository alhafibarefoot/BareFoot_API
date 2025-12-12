using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;
using MinAPI.Validations;
using static MinAPI.Data.DTOs.PostDTOs;

namespace MinAPI.EndPoints
{
    public static class AutoMapper
    {
        public static RouteGroupBuilder MapAutoMapperPost(this RouteGroupBuilder group)
        {
            //Creating Variables of Lists
            group
                .MapGet(
                    "/automapper/posts",
                    async (IPostRepo repo, IMapper mapper, [AsParameters] MinAPI.Data.DTOs.PostQueryParameters parameters) =>
                    {
                        var varPosts = await repo.GetAllPosts(parameters);
                        return Results.Ok(mapper.Map<IEnumerable<PostReadDto>>(varPosts));
                    }
                )
                .WithDescription("return All posts news ")
                .WithSummary("احضار جميع الأخبار")
                .WithOpenApi();

            group
                .MapGet(
                    "/automapper/posts/{id}",
                    async (IPostRepo repo, IMapper mapper, int id) =>
                    {
                        var varPost = await repo.GetPostById(id);
                        if (varPost != null)
                        {
                            return Results.Ok(mapper.Map<PostReadDto>(varPost));
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
                        IPostRepo repo,
                        IMapper mapper,
                        [FromBody] PostNewOrUpdatedDto postCreateDto
                    ) =>
                    {
                        var postModel = mapper.Map<Post>(postCreateDto);
                        await repo.CreatePost(postModel);
                        await repo.SaveChanges();
                        var postReadDto = mapper.Map<PostReadDto>(postModel);
                        return Results.Created($"/automapper/posts/{postReadDto.Id}", postReadDto);
                    }
                )
                .AddEndpointFilter<ValidationFilter<PostNewOrUpdatedDto>>()
                .WithDescription("Insert New Post News")
                .WithSummary("ادخال خبر جديد ")
                .WithOpenApi();

            group
                .MapPut(
                    "/automapper/posts/{id}",
                    async (
                        IPostRepo repo,
                        IMapper mapper,
                        int id,
                        PostNewOrUpdatedDto postUpdateDto
                    ) =>
                    {
                        var varPost = await repo.GetPostById(id);
                        if (varPost == null)
                        {
                            return Results.NotFound();
                        }
                        mapper.Map(postUpdateDto, varPost);
                        await repo.SaveChanges();
                        return Results.NoContent();
                    }
                )
                .WithDescription("Update Post News")
                .WithSummary("تعديل خبر  ")
                .WithOpenApi();

            group
                .MapDelete(
                    "/automapper/posts/{id}",
                    async (IPostRepo repo, IMapper mapper, int id) =>
                    {
                        var varPost = await repo.GetPostById(id);
                        if (varPost == null)
                        {
                            return Results.NotFound();
                        }
                        repo.DeletePost(varPost);
                        await repo.SaveChanges();
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
