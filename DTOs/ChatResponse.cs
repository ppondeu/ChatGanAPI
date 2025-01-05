using System;

namespace ChatApi.DTOs;

public class ChatResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid CreatorId { get; set; }
    public UserResponse? Creator { get; set; }
    public MessageResponse? LastMessage { get; set; }
    public ICollection<UserResponse>? Members { get; set; }
    public ICollection<MessageResponse>? Messages { get; set; }
}
