using MinAPI.Data.Models;

namespace MinAPI.Data.Interfaces
{
    public interface IPostRepo
    {
        Task SaveChanges();
        Task<Post?> GetPostById(int id);
        Task<IEnumerable<Post>> GetAllPosts();
        Task CreatePost(Post pst);
        void DeletePost(Post pst);
    }
}
