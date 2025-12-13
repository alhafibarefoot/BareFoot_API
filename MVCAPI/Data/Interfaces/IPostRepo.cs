using MVCAPI.Data.Models;

namespace MVCAPI.Data.Interfaces
{
    public interface IPostRepo
    {
        Task SaveChanges();
        Task<Post?> GetPostById(int id);
        Task<IEnumerable<Post>> GetAllPosts(MVCAPI.Data.DTOs.PostQueryParameters parameters);
        Task CreatePost(Post pst);
        void DeletePost(Post pst);
    }
}
