using Microsoft.EntityFrameworkCore;
using MinAPI.Data.Model;

namespace MinAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }
    }
}
