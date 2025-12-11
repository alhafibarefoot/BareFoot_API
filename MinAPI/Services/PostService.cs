using AutoMapper;
using MinAPI.Data.DTOs;
using MinAPI.Data.Interfaces;
using MinAPI.Data.Models;
using MinAPI.Services.Interfaces;
using static MinAPI.Data.DTOs.PostDTOs;

namespace MinAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepo _repo;
        private readonly IMapper _mapper;

        public PostService(IPostRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync()
        {
            var posts = await _repo.GetAllPosts();
            return _mapper.Map<IEnumerable<PostReadDto>>(posts);
        }

        public async Task<PostReadDto?> GetPostByIdAsync(int id)
        {
            var post = await _repo.GetPostById(id);
            if (post == null) return null;
            return _mapper.Map<PostReadDto>(post);
        }

        public async Task<PostReadDto> CreatePostAsync(PostNewOrUpdatedDto postDto)
        {
            var postModel = _mapper.Map<Post>(postDto);
            await _repo.CreatePost(postModel);
            await _repo.SaveChanges();
            return _mapper.Map<PostReadDto>(postModel);
        }

        public async Task<bool> UpdatePostAsync(int id, PostNewOrUpdatedDto postDto)
        {
            var post = await _repo.GetPostById(id);
            if (post == null) return false;

            _mapper.Map(postDto, post);
            await _repo.SaveChanges();
            return true;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _repo.GetPostById(id);
            if (post == null) return false;

            _repo.DeletePost(post);
            await _repo.SaveChanges();
            return true;
        }
    }
}
