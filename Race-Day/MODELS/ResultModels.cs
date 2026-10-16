using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Result
{
    [Key]
    public int ResultId { get; set; }

    [Required]
    public int EnrolmentId { get; set; }

    public TimeSpan? GunTime { get; set; }
    public TimeSpan? ChipTime { get; set; }

    public int? OverallPosition { get; set; }
    public int? CategoryPosition { get; set; }

    [MaxLength(20)]
    public string? Pace { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Provisional"; // Provisional, Official, Disqualified

    // Navigation properties
    [ForeignKey(nameof(EnrolmentId))]
    public Enrolment Enrolment { get; set; } = null!;
}