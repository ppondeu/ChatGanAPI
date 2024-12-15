using System;
using ChatApi.Entities;

namespace ChatApi.DTOs;

public class AuthResponse()
{
    public required UserResponse User { get; set; }
    public required TokenResponse Tokens { get; set; }
}
