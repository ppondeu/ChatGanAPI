using System;

namespace ChatApi.DTOs;

public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public UserResponse? Sender { get; set; }
    public required string MessageText { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
