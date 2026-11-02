using System;
using System.Collections.Generic;

namespace RaceDay.API.DTOs
{
    public class EventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public string OrganiserName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CreateEventDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string Status { get; set; } = "Draft";
    }

    public class UpdateEventDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string? Status { get; set; }
    }
}