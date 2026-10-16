using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public class CreateCategoryDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0.1, 1000)]
    public decimal DistanceKm { get; set; }

    [Required, Range(0, 100000)]
    public decimal EntryFee { get; set; }

    [Required, Range(1, 1000000)]
    public int MaxParticipants { get; set; }
}

public class UpdateCategoryDto
{
    public string? Name { get; set; }
    public decimal? DistanceKm { get; set; }
    public decimal? EntryFee { get; set; }
    public int? MaxParticipants { get; set; }
}

// Keep this ONLY if you delete the same class from EventDTOs.cs
public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public decimal EntryFee { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentEnrolments { get; set; }
}