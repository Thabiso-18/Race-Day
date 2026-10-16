// DTOs/Enrolments/UpdateEnrolmentDto.cs
namespace RaceDay.API.DTOs.Enrolments;

public class UpdateEnrolmentDto
{
    public int? CategoryId { get; set; }
    public string? Status { get; set; }
    public int? BibNumber { get; set; }
    public string? PaymentStatus { get; set; }
}