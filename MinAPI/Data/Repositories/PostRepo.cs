using Microsoft.EntityFrameworkCore;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;

namespace MinAPI.Data.Repositories
{
    public class PostRepo : IPostRepo
    {
        private readonly AppDbContext _context;

        public PostRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePost(Post pst)
        {
            if (pst == null)
            {
                throw new ArgumentNullException(nameof(pst));
            }
            await _context.AddAsync(pst);
        }

        public void DeletePost(Post pst)
        {
            if (pst == null)
            {
                throw new ArgumentNullException(nameof(pst));
            }
            _context.Posts.Remove(pst);
        }

        public async Task<IEnumerable<Post>> GetAllPosts(MinAPI.Data.DTOs.PostQueryParameters parameters)
        {
            var query = _context.Posts.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                var search = parameters.Search.ToLower();
                query = query.Where(p =>
                    (p.Title != null && p.Title.ToLower().Contains(search))
                    || (p.Content != null && p.Content.ToLower().Contains(search))
                );
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.Sort))
            {
                var isDesc = parameters.Order?.ToLower() == "desc";
                switch (parameters.Sort.ToLower())
                {
                    case "id":
                        query = isDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id);
                        break;
                    case "title":
                        query = isDesc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                        break;
                    case "content":
                        query = isDesc ? query.OrderByDescending(p => p.Content) : query.OrderBy(p => p.Content);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // Pagination
            var page = parameters.Page ?? 1;
            var pageSize = parameters.PageSize ?? 50;
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Post?> GetPostById(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
