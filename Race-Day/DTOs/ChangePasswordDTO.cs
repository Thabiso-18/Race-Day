// DTOs/Auth/ChangePasswordDto.cs
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs.Auth;

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}