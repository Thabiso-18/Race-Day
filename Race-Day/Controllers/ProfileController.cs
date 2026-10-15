using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Profile;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
[Produces("application/json")]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ProfileController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Gets the authenticated user's profile.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var user = await _db.Users
            .Include(u => u.Participant)
            .Include(u => u.Organiser)
            .FirstOrDefaultAsync(u => u.UserId == CurrentUserId);

        if (user == null) return NotFound();

        return Ok(new ProfileResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            DateOfBirth = user.Participant?.DateOfBirth,
            IdentityNumber = user.Participant?.IdentityNumber,
            EmergencyContact = user.Participant?.EmergencyContact,
            LicenseNumber = user.Participant?.LicenseNumber,
            OrganisationName = user.Organiser?.OrganisationName,
            ContactPhone = user.Organiser?.ContactPhone
        });
    }

    /// <summary>Updates the authenticated user's profile.</summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto)
    {
        var user = await _db.Users
            .Include(u => u.Participant)
            .Include(u => u.Organiser)
            .FirstOrDefaultAsync(u => u.UserId == CurrentUserId);

        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.Name)) user.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Surname)) user.Surname = dto.Surname;
        if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;

        if (user.Participant != null)
        {
            if (dto.DateOfBirth.HasValue) user.Participant.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.EmergencyContact != null) user.Participant.EmergencyContact = dto.EmergencyContact;
            if (dto.LicenseNumber != null) user.Participant.LicenseNumber = dto.LicenseNumber;
        }

        if (user.Organiser != null)
        {
            if (dto.OrganisationName != null) user.Organiser.OrganisationName = dto.OrganisationName;
            if (dto.ContactPhone != null) user.Organiser.ContactPhone = dto.ContactPhone;
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully." });
    }
}