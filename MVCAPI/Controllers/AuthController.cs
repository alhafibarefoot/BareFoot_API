using Microsoft.AspNetCore.Mvc;
using MVCAPI.Data.DTOs;
using MVCAPI.Services.Interfaces;

namespace MVCAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// نسجيل مستخدم جدبد
        /// </summary>
        /// <param name="registerDto">Register Data</param>
        /// <returns>Result</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return BadRequest(new { message = "Registration failed." });
            }
            return Ok(result);
        }

        /// <summary>
        /// تسجيل دخول
        /// </summary>
        /// <param name="loginDto">Login Data</param>
        /// <returns>Token</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        /// <summary>
        /// Dev Token Gen
        /// </summary>
        /// <param name="dto">Secret</param>
        /// <returns>Token</returns>
        [HttpPost("dev-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDevToken([FromBody] DevSecretDto dto)
        {
            var result = await _authService.GetDevTokenAsync(dto.Secret);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }

    public record DevSecretDto(string Secret);
}
