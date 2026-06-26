using System.ComponentModel.DataAnnotations;

namespace ItLxzdbxy.WebApi.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public required string UserName { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, MinLength(6)]
    public required string Password { get; set; }

    [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfigrmPassword { get; set; }
}