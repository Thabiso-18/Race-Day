
namespace RaceDay.API.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public decimal EntryFee { get; set; }

    public int MaxParticipants { get; set; }

    public int CurrentEnrolments { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public decimal EntryFee { get; set; }

    public int MaxParticipants { get; set; }
}

public class UpdateCategoryDto
{
    public string? Name { get; set; }

    public decimal? DistanceKm { get; set; }

    public decimal? EntryFee { get; set; }

    public int? MaxParticipants { get; set; }
}