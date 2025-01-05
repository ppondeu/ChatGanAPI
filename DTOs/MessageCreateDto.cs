using ChatApi.Entities;

namespace ChatApi.DTOs;

public class MessageCreateDto
{
    public required string MessageText { get; set; }
    public required string ConnectionId { get; set; }
}
