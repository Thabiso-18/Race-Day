// Models/Weather.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Weather
{
    [Key]
    public int WeatherId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public DateTime ForecastDate { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TemperatureHigh { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TemperatureLow { get; set; }

    [MaxLength(255)]
    public string? Conditions { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? WindSpeed { get; set; }

    // Navigation properties
    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;
}