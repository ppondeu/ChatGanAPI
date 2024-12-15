using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatApi.Interfaces;

public interface IJwtService
{
    string GenerateToken(string sub, bool isAccessToken);
    JwtPayload ValidateToken(string token, bool isAccessToken);
}
