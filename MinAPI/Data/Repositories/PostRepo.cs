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

        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            return await _context.Posts.ToListAsync();
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
