// DTOs/Profile/UpdateProfileDto.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Profile;

public class UpdateProfileDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Surname { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public string? EmergencyContact { get; set; }
    public string? LicenseNumber { get; set; }
    public string? OrganisationName { get; set; }
    public string? ContactPhone { get; set; }
}