using System.ComponentModel.DataAnnotations;

namespace LxzdBxy.Backend.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    [Required, MaxLength(255)]
    public string? Email { get; private set; }
}