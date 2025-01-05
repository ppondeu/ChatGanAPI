using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.Login(loginDto);
            Response.Cookies.Append("accessToken", user.Tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Expires = DateTime.UtcNow.AddMinutes(45)
            });

            Response.Cookies.Append("refreshToken", user.Tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Expires = DateTime.UtcNow.AddHours(4)
            });

            return Ok(new ApiResponse<UserResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User logged in",
                Data = user.User
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] UserCreateDto userCreateDto)
        {
            var user = await _authService.Register(userCreateDto);
            Response.Cookies.Append("accessToken", user.Tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(45)
            });
            Response.Cookies.Append("refreshToken", user.Tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(4)
            });
            return Created("api/auth/register", new ApiResponse<UserResponse>
            {
                StatusCode = 201,
                Errors = null,
                Message = "User registered",
                Data = user.User
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> RefreshToken()
        {
            var user = HttpContext.Items["user"] as User;
            Console.WriteLine($"Existing user: {System.Text.Json.JsonSerializer.Serialize(user)}");
            Console.WriteLine($"Existing Refresh Token: {Request.Cookies["refreshToken"]}");
            if (user == null)
            {
                return Unauthorized(new ApiResponse<UserResponse>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not logged in",
                    Data = null
                });
            }
            var existingUser = await _authService.RefreshToken(user.Id, Request.Cookies["refreshToken"]);
            Response.Cookies.Append("accessToken", existingUser.Tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(45)
            });
            Response.Cookies.Append("refreshToken", existingUser.Tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(4)
            });
            return Ok(new ApiResponse<TokenResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "Token refreshed",
                Data = existingUser.Tokens
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            if (HttpContext.Items["user"] is not User user)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not logged in",
                    Data = null
                });
            }
            await _authService.Logout(user.Id, Request.Cookies["refreshToken"]);
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok(new ApiResponse<string>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User logged out",
                Data = null
            });
        }
    }
}
