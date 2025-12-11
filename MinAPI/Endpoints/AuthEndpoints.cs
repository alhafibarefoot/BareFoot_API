using Microsoft.AspNetCore.Mvc;
using MinAPI.Data.DTOs;
using MinAPI.Services.Interfaces;

namespace MinAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
        {
            group.MapPost("/register", async (IAuthService authService, [FromBody] RegisterDto registerDto) =>
            {
                var result = await authService.RegisterAsync(registerDto);
                if (result == null)
                {
                    return Results.BadRequest(new { message = "Registration failed." });
                }
                return Results.Ok(result);
            })
            .WithSummary("نسجيل مستخدم جدبد")
            .WithDescription("Register a new user");

            group.MapPost("/login", async (IAuthService authService, [FromBody] LoginDto loginDto) =>
            {
                var result = await authService.LoginAsync(loginDto);
                if (result == null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(result);
            })
            .WithSummary("تسجيل دخول")
            .WithDescription("Login with existing user");

            group.MapPost("/dev-token", async (IAuthService authService, [FromBody] DevSecretDto dto) =>
            {
                var result = await authService.GetDevTokenAsync(dto.Secret);
                if (result == null)
                {
                    return Results.Unauthorized();
                }
                return Results.Ok(result);
            })
            .WithSummary("Dev Token Gen")
            .WithDescription("Generate token using secret phrase");

            return group;
        }
    }

    public record DevSecretDto(string Secret);
}
