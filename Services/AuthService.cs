using System.IdentityModel.Tokens.Jwt;
using ChatApi.Common.Exceptions;
using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Interfaces;

namespace ChatApi.Services;

public class AuthService(IUserService userService, IJwtService jwtService, IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<AuthResponse> Login(LoginDto loginDto)
    {
        var user = await _userService.GetUserByEmail(loginDto.Email);
        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.Password))
        {
            throw new BadRequestError("Invalid email or password");
        }

        string accessToken = _jwtService.GenerateToken(user.Id.ToString(), true);
        string refreshToken = _jwtService.GenerateToken(user.Id.ToString(), false);
        await _userService.UpdateRefreshToken(user.Id, refreshToken);

        return new AuthResponse
        {
            User = new UserResponse(user),
            Tokens = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<AuthResponse> Register(UserCreateDto userCreateDto)
    {
        var user = await _userService.CreateUser(userCreateDto);

        string accessToken = _jwtService.GenerateToken(user.Id.ToString(), true);
        string refreshToken = _jwtService.GenerateToken(user.Id.ToString(), false);
        await _userService.UpdateRefreshToken(user.Id, refreshToken);
        return new AuthResponse
        {
            User = new UserResponse(user),
            Tokens = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<AuthResponse> RefreshToken(Guid userId, string? refreshToken)
    {
        var user = await _userService.GetUserById(userId);
        if (user.RefreshToken != refreshToken)
        {
            throw new BadRequestError("Invalid refresh token");
        }

        string accessToken = _jwtService.GenerateToken(user.Id.ToString(), true);
        string newRefreshToken = _jwtService.GenerateToken(user.Id.ToString(), false);
        await _userService.UpdateRefreshToken(user.Id, newRefreshToken);

        return new AuthResponse
        {
            User = new UserResponse(user),
            Tokens = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            }
        };
    }

    public async Task Logout(Guid userId, string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new BadRequestError("Refresh token is required");
        }

        var user = await _userService.GetUserById(userId);

        if (user.RefreshToken != refreshToken)
        {
            throw new BadRequestError("Invalid refresh token");
        }

        await _userService.UpdateRefreshToken(user.Id, null);
    }

    public async Task<User> ValidateToken(string token, bool isAccessToken)
    {
        var tokenData = _jwtService.ValidateToken(token, isAccessToken);

        var subClaim = tokenData.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);

        if (subClaim == null)
        {
            throw new InvalidOperationException("Token does not contain a valid 'sub' claim.");
        }

        var userId = Guid.Parse(subClaim.Value);
        var user = await _userService.GetUserById(userId);

        return user;
    }

}
