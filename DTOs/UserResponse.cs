using ChatApi.Entities;

namespace ChatApi.DTOs;
public class UserResponse(User user)
{
    public Guid Id { get; set; } = user.Id;
    public string Username { get; set; } = user.Username;
    public string Email { get; set; } = user.Email;
    public string? FirstName { get; set; } = user.FirstName;
    public string? LastName { get; set; } = user.LastName;
    public DateTime CreatedAt { get; set; } = user.CreatedAt;
    public DateTime? DeletedAt { get; set; }
    public string? Avatar { get; set; } = user.Avatar;
}