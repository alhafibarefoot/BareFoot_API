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
        private readonly IWebHostEnvironment _environment;

        public PostService(IPostRepo repo, IMapper mapper, IWebHostEnvironment environment)
        {
            _repo = repo;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<IEnumerable<PostReadDto>> GetAllPostsAsync(MinAPI.Data.DTOs.PostQueryParameters parameters)
        {
            var posts = await _repo.GetAllPosts(parameters);
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
            await _repo.SaveChanges(); // Get ID

            if (postDto.Image != null)
            {
                var fileExtension = Path.GetExtension(postDto.Image.FileName);
                var fileName = $"Post{postModel.Id}_{postModel.Title}{fileExtension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "Posts");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.Image.CopyToAsync(stream);
                }

                postModel.postImage = $"img/Posts/{fileName}";
                await _repo.SaveChanges();
            }

            return _mapper.Map<PostReadDto>(postModel);
        }

        public async Task<bool> UpdatePostAsync(int id, PostNewOrUpdatedDto postDto)
        {
            var post = await _repo.GetPostById(id);
            if (post == null) return false;

            _mapper.Map(postDto, post);

            if (postDto.Image != null)
            {
                var fileExtension = Path.GetExtension(postDto.Image.FileName);
                var fileName = $"Post{post.Id}_{post.Title}{fileExtension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "Posts");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.Image.CopyToAsync(stream);
                }

                post.postImage = $"img/Posts/{fileName}";
            }

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
