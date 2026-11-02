namespace RaceDay.MVC.Models
{
    // Maps to the Event object from your API
    public class EventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public decimal EntryFee { get; set; }
    }

    // Maps to the payload sent when creating an event
    public class CreateEventDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string Status { get; set; } = "Open";
    }

    // Maps to the Enrolment/History object from your API
    public class EnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? FinishTime { get; set; }
        public string? Position { get; set; }
    }
}