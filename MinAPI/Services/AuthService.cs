using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MinAPI.Data.DTOs;
using MinAPI.Services.Interfaces;

namespace MinAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<UserDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return null;

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<UserDto?> RegisterAsync(RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                // For simplicity, we return null if registration fails,
                // in a real app, you might want to return errors.
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Token = GenerateJwtToken(user)
            };
        }

        public Task<UserDto?> GetDevTokenAsync(string secret)
        {
            if (secret != "barefoot2020") return Task.FromResult<UserDto?>(null);

            var devUser = new IdentityUser
            {
                Id = "dev-user-id",
                Email = "dev@barefoot.com",
                UserName = "devuser"
            };

            var userDto = new UserDto
            {
                Id = devUser.Id,
                Email = devUser.Email,
                Token = GenerateJwtToken(devUser)
            };

            return Task.FromResult<UserDto?>(userDto);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
