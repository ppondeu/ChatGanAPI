using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.DTOs;

public class ChatCreateDto
{
    [Required]
    [StringLength(32, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 1)]
    public required string ConnectionId { get; set; }
    public IFormFile? Image { get; set; }
    public required IEnumerable<string> OtherMemberIds { get; set; }
}

