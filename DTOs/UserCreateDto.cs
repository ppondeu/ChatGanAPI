using System.ComponentModel.DataAnnotations;

namespace ChatApi.DTOs;

public class UserCreateDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(32, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
    [RegularExpression("^[a-z0-9_]+$", ErrorMessage = "Username can only contain lowercase letters, numbers, and underscores.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password can't be shorter than 8 characters")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public required string Email { get; set; }
}
