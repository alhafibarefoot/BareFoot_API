using MinAPI.Data.DTOs;

namespace MinAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto?> RegisterAsync(RegisterDto registerDto);
        Task<UserDto?> LoginAsync(LoginDto loginDto);
    }
}
