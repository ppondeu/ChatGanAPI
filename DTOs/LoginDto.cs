using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.DTOs;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }

    [MinLength(8, ErrorMessage = "Password can't be shorter than 8 characters.")]
    public required string Password { get; set; }
}
