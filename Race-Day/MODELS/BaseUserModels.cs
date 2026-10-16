using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Organiser? Organiser { get; set; }
    public Participant? Participant { get; set; }
}