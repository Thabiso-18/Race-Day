// DTOs/Events/CreateEventDto.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Events;

public class CreateEventDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public DateTime RegistrationDeadline { get; set; }

    public string Status { get; set; } = "Draft";
}