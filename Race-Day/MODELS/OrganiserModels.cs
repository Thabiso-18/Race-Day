// Models/Organiser.cs
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Organiser
{
    [Key]
    public int OrganiserId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string OrganisationName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public ICollection<Event> Events { get; set; } = new List<Event>();
}