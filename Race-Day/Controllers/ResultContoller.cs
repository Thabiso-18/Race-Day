using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Results;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ResultsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Adds or updates a result for an enrolment. Organiser only.</summary>
    [HttpPost("enrolments/{enrolmentId:int}/result")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Upsert(int enrolmentId, [FromBody] CreateResultDto dto)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var enrolment = await _db.Enrolments
            .Include(e => e.Category).ThenInclude(c => c.Event)
            .Include(e => e.Result)
            .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);
        if (enrolment == null) return NotFound();
        if (enrolment.Category.Event.OrganiserId != organiser.OrganiserId) return Forbid();

        if (enrolment.Result == null)
        {
            enrolment.Result = new Result
            {
                EnrolmentId = enrolmentId,
                GunTime = dto.GunTime,
                ChipTime = dto.ChipTime,
                OverallPosition = dto.OverallPosition,
                CategoryPosition = dto.CategoryPosition,
                Pace = dto.Pace,
                Status = dto.Status
            };
        }
        else
        {
            enrolment.Result.GunTime = dto.GunTime ?? enrolment.Result.GunTime;
            enrolment.Result.ChipTime = dto.ChipTime ?? enrolment.Result.ChipTime;
            enrolment.Result.OverallPosition = dto.OverallPosition ?? enrolment.Result.OverallPosition;
            enrolment.Result.CategoryPosition = dto.CategoryPosition ?? enrolment.Result.CategoryPosition;
            enrolment.Result.Pace = dto.Pace ?? enrolment.Result.Pace;
            enrolment.Result.Status = dto.Status;
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Result saved." });
    }

    /// <summary>Gets all published results for an event.</summary>
    [HttpGet("events/{eventId:int}/results")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        var results = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Participant).ThenInclude(p => p.User)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.Category.EventId == eventId && r.Status == "Official")
            .Select(r => new
            {
                r.ResultId,
                r.GunTime,
                r.ChipTime,
                r.OverallPosition,
                r.CategoryPosition,
                r.Pace,
                r.Status,
                ParticipantName = $"{r.Enrolment.Participant.User.Name} {r.Enrolment.Participant.User.Surname}",
                CategoryName = r.Enrolment.Category.Name
            })
            .ToListAsync();

        return Ok(results);
    }
}