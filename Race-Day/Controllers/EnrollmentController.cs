using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Enrolments;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
[Produces("application/json")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role)!;

    /// <summary>Gets enrolments for an event. Organiser sees all; Participant sees own.</summary>
    [HttpGet("events/{eventId:int}/enrolments")]
    [ProducesResponseType(typeof(List<EnrolmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var query = _db.Enrolments
            .Include(e => e.Participant).ThenInclude(p => p.User)
            .Include(e => e.Category).ThenInclude(c => c.Event)
            .Include(e => e.Result)
            .Where(e => e.Category.EventId == eventId);

        if (CurrentUserRole == "Participant")
        {
            var participant = await _db.Participants.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
            if (participant == null) return Forbid();
            query = query.Where(e => e.ParticipantId == participant.ParticipantId);
        }
        else
        {
            var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
            if (organiser == null ||
                !await _db.Events.AnyAsync(e => e.EventId == eventId && e.OrganiserId == organiser.OrganiserId))
                return Forbid();
        }

        var items = await query.Select(e => Map(e)).ToListAsync();
        return Ok(items);
    }

    /// <summary>Enrols a participant in a category.</summary>
    [HttpPost("events/{eventId:int}/enrol")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enrol(int eventId, [FromBody] CreateEnrolmentDto dto)
    {
        var participant = await _db.Participants.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
        if (participant == null) return Forbid();

        var category = await _db.Categories
            .Include(c => c.Event)
            .Include(c => c.Enrolments)
            .FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId && c.EventId == eventId);
        if (category == null) return NotFound(new { message = "Category not found." });

        if (category.Event.Status != "Open")
            return BadRequest(new { message = "Event is not open for registration." });
        if (DateTime.UtcNow > category.Event.RegistrationDeadline)
            return BadRequest(new { message = "Registration deadline has passed." });
        if (category.Enrolments.Count >= category.MaxParticipants)
            return BadRequest(new { message = "Category is full." });

        if (await _db.Enrolments.AnyAsync(e =>
            e.ParticipantId == participant.ParticipantId && e.CategoryId == dto.CategoryId))
            return Conflict(new { message = "You are already enrolled in this category." });

        var enrolment = new Enrolment
        {
            ParticipantId = participant.ParticipantId,
            CategoryId = dto.CategoryId,
            Status = "Pending",
            PaymentStatus = "Unpaid"
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created,
            new { enrolment.EnrolmentId, enrolment.Status });
    }

    /// <summary>Updates an enrolment. Participant changes category; Organiser updates status/bib.</summary>
    [HttpPut("enrolments/{enrolmentId:int}")]
    public async Task<IActionResult> Update(int enrolmentId, [FromBody] UpdateEnrolmentDto dto)
    {
        var enrolment = await _db.Enrolments
            .Include(e => e.Category).ThenInclude(c => c.Event)
            .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);
        if (enrolment == null) return NotFound();

        if (CurrentUserRole == "Participant")
        {
            var participant = await _db.Participants.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
            if (participant == null || enrolment.ParticipantId != participant.ParticipantId) return Forbid();

            if (dto.CategoryId.HasValue) enrolment.CategoryId = dto.CategoryId.Value;
        }
        else
        {
            var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
            if (organiser == null || enrolment.Category.Event.OrganiserId != organiser.OrganiserId) return Forbid();

            if (dto.Status != null) enrolment.Status = dto.Status;
            if (dto.BibNumber.HasValue) enrolment.BibNumber = dto.BibNumber.Value;
            if (dto.PaymentStatus != null) enrolment.PaymentStatus = dto.PaymentStatus;
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Enrolment updated." });
    }

    /// <summary>Cancels own enrolment.</summary>
    [HttpDelete("enrolments/{enrolmentId:int}")]
    [Authorize(Roles = "Participant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(int enrolmentId)
    {
        var participant = await _db.Participants.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
        if (participant == null) return Forbid();

        var enrolment = await _db.Enrolments
            .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId
                                   && e.ParticipantId == participant.ParticipantId);
        if (enrolment == null) return NotFound();

        _db.Enrolments.Remove(enrolment);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static EnrolmentDto Map(Enrolment e) => new()
    {
        EnrolmentId = e.EnrolmentId,
        ParticipantId = e.ParticipantId,
        ParticipantName = $"{e.Participant.User.Name} {e.Participant.User.Surname}",
        CategoryId = e.CategoryId,
        CategoryName = e.Category.Name,
        EventName = e.Category.Event.Name,
        EnrolmentDate = e.EnrolmentDate,
        Status = e.Status,
        BibNumber = e.BibNumber,
        PaymentStatus = e.PaymentStatus,
        Result = e.Result == null ? null : new ResultDto
        {
            ResultId = e.Result.ResultId,
            GunTime = e.Result.GunTime,
            ChipTime = e.Result.ChipTime,
            OverallPosition = e.Result.OverallPosition,
            CategoryPosition = e.Result.CategoryPosition,
            Pace = e.Result.Pace,
            Status = e.Result.Status
        }
    };
}