using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Event
{
    [Key]
    public int EventId { get; set; }

    [Required]
    public int OrganiserId { get; set; }

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

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Open, Closed, Completed

    // Navigation properties
    [ForeignKey(nameof(OrganiserId))]
    public Organiser Organiser { get; set; } = null!;

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Weather> WeatherData { get; set; } = new List<Weather>();
    public ICollection<Route> Routes { get; set; } = new List<Route>();
}