using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatApi.Common.Exceptions;
using ChatApi.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ChatApi.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(string sub, bool isAccessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = isAccessToken ? _configuration["Jwt:AccessSecret"] : _configuration["Jwt:RefreshSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured.");
        }
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new(JwtRegisteredClaimNames.Sub, sub),
            ]),
            Expires = DateTime.UtcNow.AddHours(isAccessToken ? 1 : 24),
            NotBefore = isAccessToken ? DateTime.UtcNow : DateTime.UtcNow.AddSeconds(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public JwtPayload ValidateToken(string token, bool isAccessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = isAccessToken ? _configuration["Jwt:AccessSecret"] : _configuration["Jwt:RefreshSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured.");
        }
        var key = Encoding.ASCII.GetBytes(secret);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return ((JwtSecurityToken)validatedToken).Payload;
        }
        catch (Exception)
        {
            throw new UnauthorizedError("Invalid token");
        }
    }

}
