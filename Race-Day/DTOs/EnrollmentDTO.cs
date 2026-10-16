// DTOs/Enrolments/EnrolmentDto.cs
namespace RaceDay.API.DTOs.Enrolments;

public class EnrolmentDto
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EnrolmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? BibNumber { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public ResultDto? Result { get; set; }
}

public class ResultDto
{
    public int ResultId { get; set; }
    public TimeSpan? GunTime { get; set; }
    public TimeSpan? ChipTime { get; set; }
    public int? OverallPosition { get; set; }
    public int? CategoryPosition { get; set; }
    public string? Pace { get; set; }
    public string Status { get; set; } = string.Empty;
}