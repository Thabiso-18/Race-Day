using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Events;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1/events/{eventId:int}/weather")]
[Produces("application/json")]
public class WeatherController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public WeatherController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Gets weather data for an event.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<WeatherDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int eventId)
    {
        var weather = await _db.WeatherData
            .Where(w => w.EventId == eventId)
            .Select(w => new WeatherDto
            {
                WeatherId = w.WeatherId,
                ForecastDate = w.ForecastDate,
                TemperatureHigh = w.TemperatureHigh,
                TemperatureLow = w.TemperatureLow,
                Conditions = w.Conditions,
                WindSpeed = w.WindSpeed
            })
            .ToListAsync();

        return Ok(weather);
    }

    /// <summary>Adds or updates weather for an event. Organiser only.</summary>
    [HttpPut]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upsert(int eventId, [FromBody] WeatherDto dto)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var ev = await _db.Events.FirstOrDefaultAsync(e =>
            e.EventId == eventId && e.OrganiserId == organiser.OrganiserId);
        if (ev == null) return NotFound();

        var existing = await _db.WeatherData
            .FirstOrDefaultAsync(w => w.EventId == eventId && w.ForecastDate == dto.ForecastDate);

        if (existing == null)
        {
            _db.WeatherData.Add(new Weather
            {
                EventId = eventId,
                ForecastDate = dto.ForecastDate,
                TemperatureHigh = dto.TemperatureHigh,
                TemperatureLow = dto.TemperatureLow,
                Conditions = dto.Conditions,
                WindSpeed = dto.WindSpeed
            });
        }
        else
        {
            existing.TemperatureHigh = dto.TemperatureHigh;
            existing.TemperatureLow = dto.TemperatureLow;
            existing.Conditions = dto.Conditions;
            existing.WindSpeed = dto.WindSpeed;
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Weather data saved successfully." });
    }
}