// Models/Participant.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Participant
{
    [Key]
    public int ParticipantId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(13)]
    public string IdentityNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? EmergencyContact { get; set; }

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}