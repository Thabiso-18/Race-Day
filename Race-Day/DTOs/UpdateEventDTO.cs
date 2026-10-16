// DTOs/Events/UpdateEventDto.cs
namespace RaceDay.API.DTOs.Events;

public class UpdateEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime? EventDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public string? Status { get; set; }
}