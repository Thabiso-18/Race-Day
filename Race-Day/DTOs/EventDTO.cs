// DTOs/Events/EventDto.cs
namespace RaceDay.API.DTOs.Events;

public class EventDto
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OrganiserName { get; set; } = string.Empty;
    public List<CategoryDto> Categories { get; set; } = new();
    public RouteDto? Route { get; set; }
    public List<WeatherDto> Weather { get; set; } = new();
}

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public decimal EntryFee { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentEnrolments { get; set; }
}

public class RouteDto
{
    public int RouteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GpxFileUrl { get; set; }
    public decimal? ElevationGain { get; set; }
    public decimal? ElevationLoss { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
}

public class WeatherDto
{
    public int WeatherId { get; set; }
    public DateTime ForecastDate { get; set; }
    public decimal? TemperatureHigh { get; set; }
    public decimal? TemperatureLow { get; set; }
    public string? Conditions { get; set; }
    public decimal? WindSpeed { get; set; }
}