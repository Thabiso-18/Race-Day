using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Waitlisted, DNS, DNF, Finished

    public int? BibNumber { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Refunded

    // Navigation properties
    [ForeignKey(nameof(ParticipantId))]
    public Participant Participant { get; set; } = null!;

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    public Result? Result { get; set; }
}