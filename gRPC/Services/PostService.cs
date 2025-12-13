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
