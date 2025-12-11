using MinAPI.Data.DTOs;
using static MinAPI.Data.DTOs.PostDTOs;

namespace MinAPI.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostReadDto>> GetAllPostsAsync(MinAPI.Data.DTOs.PostQueryParameters parameters);
        Task<PostReadDto?> GetPostByIdAsync(int id);
        Task<PostReadDto> CreatePostAsync(PostNewOrUpdatedDto postDto);
        Task<bool> UpdatePostAsync(int id, PostNewOrUpdatedDto postDto);
        Task<bool> DeletePostAsync(int id);
    }
}
