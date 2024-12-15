using ChatApi.DTOs;
using ChatApi.Entities;

namespace ChatApi.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginDto loginDto);
    Task<AuthResponse> Register(UserCreateDto userCreateDto);
    Task<AuthResponse> RefreshToken(Guid userId, string? refreshToken);
    Task Logout(Guid userId, string? refreshToken);
    Task<User> ValidateToken(string token, bool isAccessToken);
}
