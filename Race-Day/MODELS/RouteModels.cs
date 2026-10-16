
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Route
{
    [Key]
    public int RouteId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? GpxFileUrl { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ElevationGain { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ElevationLoss { get; set; }

    [MaxLength(255)]
    public string? StartPoint { get; set; }

    [MaxLength(255)]
    public string? EndPoint { get; set; }

    // Navigation properties
    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;
}