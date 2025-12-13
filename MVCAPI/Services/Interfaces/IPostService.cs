using MVCAPI.Data.DTOs;
using static MVCAPI.Data.DTOs.PostDTOs;

namespace MVCAPI.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostReadDto>> GetAllPostsAsync(MVCAPI.Data.DTOs.PostQueryParameters parameters);
        Task<PostReadDto?> GetPostByIdAsync(int id);
        Task<PostReadDto> CreatePostAsync(PostNewOrUpdatedDto postDto);
        Task<bool> UpdatePostAsync(int id, PostNewOrUpdatedDto postDto);
        Task<bool> DeletePostAsync(int id);
    }
}
