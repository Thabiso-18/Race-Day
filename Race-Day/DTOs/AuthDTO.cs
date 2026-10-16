// DTOs/Auth/RegisterRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

    // Participant-specific
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public string? LicenseNumber { get; set; }

    // Organiser-specific
    public string? OrganisationName { get; set; }
    public string? ContactPhone { get; set; }
}