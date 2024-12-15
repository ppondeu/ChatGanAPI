using ChatApi.DTOs;
using ChatApi.Interfaces;

namespace ChatApi.Middlewares;

public class JwtRefreshTokenMiddleware(IAuthService authService) : IMiddleware
{
    private readonly IAuthService _authService = authService;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var refreshToken = context.Request.Cookies["refreshToken"];
        Console.WriteLine($"Refresh Token: {refreshToken}");
        if (refreshToken == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                StatusCode = 401,
                Errors = ["Unauthorized"],
                Message = "Unauthorized",
                Data = null
            });
            return;
        }
        try
        {
            var user = await _authService.ValidateToken(refreshToken, false);
            context.Items["user"] = user;
        }
        catch (Exception)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>
            {
                StatusCode = 401,
                Errors = ["Unauthorized"],
                Message = "Unauthorized",
                Data = null
            });
            return;
        }

        await next(context);
    }
}
