using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Events;
using RaceDay.API.Models;
using EventCategoryDto = RaceDay.API.DTOs.Events.CategoryDto;
using System.Security.Claims;


namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1/events")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public EventsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponseDto<EventDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status,
                                            [FromQuery] int page = 1,
                                            [FromQuery] int pageSize = 10)
    {
        var query = _db.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories).ThenInclude(c => c.Enrolments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(e => e.Status == status);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.EventDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                Location = e.Location,
                EventDate = e.EventDate,
                RegistrationDeadline = e.RegistrationDeadline,
                Status = e.Status,
                OrganiserName = e.Organiser.OrganisationName,
                Categories = e.Categories.Select(c => new RaceDay.API.DTOs.Events.CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    DistanceKm = c.DistanceKm,
                    EntryFee = c.EntryFee,
                    MaxParticipants = c.MaxParticipants,
                    CurrentEnrolments = c.Enrolments.Count
                }).ToList()
            })
            .ToListAsync();

        return Ok(new PagedResponseDto<EventDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    
    [HttpGet("{eventId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int eventId)
    {
        var e = await _db.Events
            .Include(x => x.Organiser)
            .Include(x => x.Categories).ThenInclude(c => c.Enrolments)
            .Include(x => x.Routes)
            .Include(x => x.WeatherData)
            .FirstOrDefaultAsync(x => x.EventId == eventId);

        if (e == null) return NotFound();

        return Ok(new EventDto
        {
            EventId = e.EventId,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            EventDate = e.EventDate,
            RegistrationDeadline = e.RegistrationDeadline,
            Status = e.Status,
            OrganiserName = e.Organiser.OrganisationName,
            Categories = e.Categories.Select(c => new EventCategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                DistanceKm = c.DistanceKm,
                EntryFee = c.EntryFee,
                MaxParticipants = c.MaxParticipants,
                CurrentEnrolments = c.Enrolments.Count
            }).ToList(),
            Route = e.Routes.Select(r => new RouteDto
            {
                RouteId = r.RouteId,
                Name = r.Name,
                GpxFileUrl = r.GpxFileUrl,
                ElevationGain = r.ElevationGain,
                ElevationLoss = r.ElevationLoss,
                StartPoint = r.StartPoint,
                EndPoint = r.EndPoint
            }).FirstOrDefault(),
            Weather = e.WeatherData.Select(w => new WeatherDto
            {
                WeatherId = w.WeatherId,
                ForecastDate = w.ForecastDate,
                TemperatureHigh = w.TemperatureHigh,
                TemperatureLow = w.TemperatureLow,
                Conditions = w.Conditions,
                WindSpeed = w.WindSpeed
            }).ToList()
        });
    }

    /// <summary>Creates a new event. Organiser only.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var ev = new Event
        {
            OrganiserId = organiser.OrganiserId,
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            EventDate = dto.EventDate,
            RegistrationDeadline = dto.RegistrationDeadline,
            Status = dto.Status
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { eventId = ev.EventId },
            new { ev.EventId, ev.Name, ev.Status });
    }

    /// <summary>Updates an existing event. Organiser only.</summary>
    [HttpPut("{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int eventId, [FromBody] UpdateEventDto dto)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var ev = await _db.Events.FirstOrDefaultAsync(e =>
            e.EventId == eventId && e.OrganiserId == organiser.OrganiserId);
        if (ev == null) return NotFound();

        if (dto.Name != null) ev.Name = dto.Name;
        if (dto.Description != null) ev.Description = dto.Description;
        if (dto.Location != null) ev.Location = dto.Location;
        if (dto.EventDate.HasValue) ev.EventDate = dto.EventDate.Value;
        if (dto.RegistrationDeadline.HasValue) ev.RegistrationDeadline = dto.RegistrationDeadline.Value;
        if (dto.Status != null) ev.Status = dto.Status;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Event updated successfully." });
    }

    /// <summary>Deletes an event. Organiser only, no enrolments allowed.</summary>
    [HttpDelete("{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int eventId)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var ev = await _db.Events
            .Include(e => e.Categories).ThenInclude(c => c.Enrolments)
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.OrganiserId == organiser.OrganiserId);

        if (ev == null) return NotFound();
        if (ev.Categories.Any(c => c.Enrolments.Any()))
            return Conflict(new { message = "Cannot delete event with existing enrolments." });

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Gets upcoming events (next 30 days).</summary>
    [HttpGet("upcoming")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Upcoming()
    {
        var events = await _db.Events
            .Where(e => e.EventDate >= DateTime.UtcNow && e.EventDate <= DateTime.UtcNow.AddDays(30))
            .OrderBy(e => e.EventDate)
            .Select(e => new { e.EventId, e.Name, e.Location, e.EventDate, e.Status })
            .ToListAsync();

        return Ok(events);
    }
}
