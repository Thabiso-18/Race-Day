// DTOs/Profile/ProfileResponseDto.cs
namespace RaceDay.API.DTOs.Profile;

public class ProfileResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Participant-specific
    public DateTime? DateOfBirth { get; set; }
    public string? IdentityNumber { get; set; }
    public string? EmergencyContact { get; set; }
    public string? LicenseNumber { get; set; }

    // Organiser-specific
    public string? OrganisationName { get; set; }
    public string? ContactPhone { get; set; }
}