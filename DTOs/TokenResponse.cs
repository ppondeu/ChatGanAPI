using System;

namespace ChatApi.DTOs;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
