// DTOs/Results/CreateResultDto.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Results;

public class CreateResultDto
{
    public TimeSpan? GunTime { get; set; }
    public TimeSpan? ChipTime { get; set; }
    public int? OverallPosition { get; set; }
    public int? CategoryPosition { get; set; }
    public string? Pace { get; set; }
    public string Status { get; set; } = "Provisional";
}