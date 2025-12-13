using Grpc.Core;
using gRPC.Data;
using gRPC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPC.Services
{
    public class PostService : gRPC.PostService.PostServiceBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostService> _logger;

        public PostService(AppDbContext context, ILogger<PostService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<PostModel> CreatePost(CreatePostRequest request, ServerCallContext context)
        {
            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                CreatedOn = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created post with ID: {PostId}", post.Id);

            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedOn = post.CreatedOn.ToString()
            };
        }

        public override async Task<PostModel> ReadPost(ReadPostRequest request, ServerCallContext context)
        {
            var post = await _context.Posts.FindAsync(request.Id);

            if (post == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Post with ID={request.Id} is not found."));
            }

            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedOn = post.CreatedOn.ToString()
            };
        }

        public override async Task<PostModel> UpdatePost(UpdatePostRequest request, ServerCallContext context)
        {
            var post = await _context.Posts.FindAsync(request.Id);

            if (post == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Post with ID={request.Id} is not found."));
            }

            // Update fields
            post.Title = request.Title;
            post.Content = request.Content;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated post with ID: {PostId}", post.Id);

            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedOn = post.CreatedOn.ToString()
            };
        }

        public override async Task<DeletePostReply> DeletePost(DeletePostRequest request, ServerCallContext context)
        {
            var post = await _context.Posts.FindAsync(request.Id);

            if (post == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Post with ID={request.Id} is not found."));
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted post with ID: {PostId}", request.Id);

            return new DeletePostReply
            {
                Success = true,
                Message = $"Post with ID={request.Id} has been deleted successfully."
            };
        }

        public override async Task<ListPostsReply> ListPosts(ListPostsRequest request, ServerCallContext context)
        {
             var posts = await _context.Posts.Select(p => new PostModel
             {
                 Id = p.Id,
                 Title = p.Title,
                 Content = p.Content,
                 CreatedOn = p.CreatedOn.ToString()
             }).ToListAsync();

             var reply = new ListPostsReply();
             reply.Posts.AddRange(posts);
             return reply;
        }
    }
}
