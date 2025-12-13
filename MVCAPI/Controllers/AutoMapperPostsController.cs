using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCAPI.Data.DTOs;
using MVCAPI.Services.Interfaces;
using MVCAPI.Validations;

namespace MVCAPI.Controllers
{
    [ApiController]
    [Route("automapper/posts")]
    [Tags("AutoMapperPostNews")]
    public class AutoMapperPostsController : ControllerBase
    {
        private readonly IPostService _service;

        public AutoMapperPostsController(IPostService service)
        {
            _service = service;
        }

        /// <summary>
        /// احضار جميع الأخبار
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPosts([FromQuery] PostQueryParameters parameters)
        {
            var posts = await _service.GetAllPostsAsync(parameters);
            return Ok(posts);
        }

        /// <summary>
        /// احضار خبر واحد
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _service.GetPostByIdAsync(id);
            if (post != null)
            {
                return Ok(post);
            }
            return NotFound();
        }

        /// <summary>
        /// ادخال خبر جديد
        /// </summary>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<PostDTOs.PostNewOrUpdatedDto>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePost([FromForm] PostDTOs.PostNewOrUpdatedDto postCreateDto)
        {
            var createdPost = await _service.CreatePostAsync(postCreateDto);
            return Created($"/automapper/posts/{createdPost.Id}", createdPost);
        }

        /// <summary>
        /// تعديل خبر
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] PostDTOs.PostNewOrUpdatedDto postUpdateDto)
        {
            var result = await _service.UpdatePostAsync(id, postUpdateDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// حذف خبر
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _service.DeletePostAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
