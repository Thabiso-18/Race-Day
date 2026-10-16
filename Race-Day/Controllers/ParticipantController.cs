using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1/participants")]
[Authorize]
[Produces("application/json")]
public class ParticipantsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public ParticipantsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role)!;

    /// <summary>Gets a participant's event history and results.</summary>
    [HttpGet("{participantId:int}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> History(int participantId)
    {
        if (CurrentUserRole == "Participant")
        {
            var own = await _db.Participants.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
            if (own == null || own.ParticipantId != participantId) return Forbid();
        }

        var history = await _db.Enrolments
            .Include(e => e.Category).ThenInclude(c => c.Event)
            .Include(e => e.Result)
            .Where(e => e.ParticipantId == participantId)
            .OrderByDescending(e => e.Category.Event.EventDate)
            .Select(e => new
            {
                e.EnrolmentId,
                Event = e.Category.Event.Name,
                Category = e.Category.Name,
                EventDate = e.Category.Event.EventDate,
                e.Status,
                e.BibNumber,
                Result = e.Result == null ? null : new
                {
                    e.Result.GunTime,
                    e.Result.ChipTime,
                    e.Result.OverallPosition,
                    e.Result.CategoryPosition,
                    e.Result.Pace,
                    e.Result.Status
                }
            })
            .ToListAsync();

        return Ok(history);
    }
}