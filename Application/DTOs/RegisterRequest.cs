using System.ComponentModel.DataAnnotations;

namespace ItLxzdbxy.WebApi.Application.DTOs;

public class RegisterDto
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, MinLength(6)]
    public required string Password { get; set; }
}