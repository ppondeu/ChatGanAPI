using System.ComponentModel.DataAnnotations;

namespace ChatApi.DTOs;

public class UserUpdateDto
{
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    [StringLength(32, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 32 characters.")]
    public string? Username { get; set; }

    [MinLength(8, ErrorMessage = "Password can't be shorter than 8 characters.")]
    public string? Password { get; set; }

    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 32 characters.")]
    public string? FirstName { get; set; }

    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 32 characters.")]
    public string? LastName { get; set; }
    public string? Avatar { get; set; }
}
