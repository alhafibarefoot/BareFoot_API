using MinAPI.Data.Models;

namespace MinAPI.Data.Interfaces
{
    public interface IPostRepo
    {
        Task SaveChanges();
        Task<Post?> GetPostById(int id);
        Task<IEnumerable<Post>> GetAllPosts(MinAPI.Data.DTOs.PostQueryParameters parameters);
        Task CreatePost(Post pst);
        void DeletePost(Post pst);
    }
}
