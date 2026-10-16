
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal DistanceKm { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal EntryFee { get; set; }

    [Required]
    public int MaxParticipants { get; set; }

    // Navigation properties
    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}