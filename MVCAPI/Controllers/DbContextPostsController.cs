using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MVCAPI.Data;
using MVCAPI.Data.Models;
using MVCAPI.Validations;
using MVCAPI.Extensions;
using FluentValidation.Results;

namespace MVCAPI.Controllers
{
    [ApiController]
    [Route("dbcontext/posts")]
    [Tags("DBContextPostNews")]
    public class DbContextPostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IValidator<Post> _validator;

        public DbContextPostsController(AppDbContext context, IOutputCacheStore outputCacheStore, IValidator<Post> validator)
        {
            _context = context;
            _outputCacheStore = outputCacheStore;
            _validator = validator;
        }

        /// <summary>
        /// احضار جميع الأخبار
        /// </summary>
        [HttpGet]
        [OutputCache(PolicyName = "PostCache")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPosts(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? order,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var query = _context.Posts.AsQueryable();

            // Filtering rules
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (p.Title != null && p.Title.ToLower().Contains(search.ToLower()))
                    || (p.Content != null && p.Content.ToLower().Contains(search.ToLower()))
                );
            }

            // Sorting rules
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "id":
                        query = order?.ToLower() == "desc" ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id);
                        break;
                    case "title":
                        query = order?.ToLower() == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                        break;
                    case "content":
                        query = order?.ToLower() == "desc" ? query.OrderByDescending(p => p.Content) : query.OrderBy(p => p.Content);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id); // Default sort
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Id); // Default sort
            }

            // Pagination rules
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

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
            var varPost = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id);
            if (varPost != null)
            {
                return Ok(varPost);
            }
            return NotFound();
        }

        /// <summary>
        /// ادخال خبر جديد
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreatePost([FromBody] Post poss)
        {
            var validationResult = await _validator.ValidateAsync(poss);

            if (validationResult.IsValid)
            {
                await _context.Posts.AddAsync(poss);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync("Post_Get", default);
                return Created($"/dbcontext/posts/{poss.Id}", poss);
            }
            return UnprocessableEntity(validationResult.ToDictionary());
        }

        /// <summary>
        /// ادخال خبر جديد V2
        /// </summary>
        [HttpPost("v2")]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilter<Post>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePostV2([FromBody] Post poss)
        {
            await _context.Posts.AddAsync(poss);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("Post_Get", default);
            return Created($"/dbcontext/posts/{poss.Id}", poss);
        }

         /// <summary>
        /// ادخال خبر جديد V3
        /// </summary>
        [HttpPost("v3")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePostV3([FromBody] Post poss)
        {
            await _context.Posts.AddAsync(poss);
            await _context.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("Post_Get", default);
            return Created($"/dbcontext/posts/{poss.Id}", poss);
        }

        /// <summary>
        /// تعديل خبر
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post poss)
        {
            var varPost = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id);
            if (varPost != null)
            {
                varPost.Title = poss.Title;
                varPost.Content = poss.Content;
                varPost.postImage = poss.postImage;
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync("Post_Get", default);
                return NoContent();
            }
            return NotFound();
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
            var varPost = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id);
            if (varPost != null)
            {
                _context.Posts.Remove(varPost);
                await _context.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync("Post_Get", default);
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// عرض خبر
        /// </summary>
        [HttpGet("display")]
        public IActionResult DisplayPost([FromQuery] string title, [FromQuery] string content)
        {
            // Note: [AsParameters] in Minimal API maps query/body/route. In MVC [FromQuery] maps query params.
            // Simplified here to strings as complex binding is different in MVC for this simple case.
             return Ok($"Title: {title}, Content: {content}");
        }
    }
}
