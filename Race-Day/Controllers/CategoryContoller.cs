using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using CategoryDto = RaceDay.API.DTOs.CategoryDto;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public CategoriesController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Gets all categories for an event.</summary>
    [HttpGet("events/{eventId:int}/categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        if (!await _db.Events.AnyAsync(e => e.EventId == eventId)) return NotFound();

        var cats = await _db.Categories
            .Where(c => c.EventId == eventId)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                DistanceKm = c.DistanceKm,
                EntryFee = c.EntryFee,
                MaxParticipants = c.MaxParticipants,
                CurrentEnrolments = c.Enrolments.Count
            })
            .ToListAsync();

        return Ok(cats);
    }

    /// <summary>Creates a new category. Organiser only.</summary>
    [HttpPost("events/{eventId:int}/categories")]
    [Authorize(Roles = "Organiser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(int eventId, [FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var ev = await _db.Events.FirstOrDefaultAsync(e =>
            e.EventId == eventId && e.OrganiserId == organiser.OrganiserId);
        if (ev == null) return NotFound();

        var cat = new Category
        {
            EventId = eventId,
            Name = dto.Name,
            DistanceKm = dto.DistanceKm,
            EntryFee = dto.EntryFee,
            MaxParticipants = dto.MaxParticipants
        };

        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new { cat.CategoryId, cat.Name });
    }

    /// <summary>Updates a category. Organiser only.</summary>
    [HttpPut("categories/{categoryId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int categoryId, [FromBody] UpdateCategoryDto dto)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var cat = await _db.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId
                                   && c.Event.OrganiserId == organiser.OrganiserId);
        if (cat == null) return NotFound();

        if (dto.Name != null) cat.Name = dto.Name;
        if (dto.DistanceKm.HasValue) cat.DistanceKm = dto.DistanceKm.Value;
        if (dto.EntryFee.HasValue) cat.EntryFee = dto.EntryFee.Value;
        if (dto.MaxParticipants.HasValue) cat.MaxParticipants = dto.MaxParticipants.Value;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Category updated." });
    }

    /// <summary>Deletes a category. Organiser only, no enrolments allowed.</summary>
    [HttpDelete("categories/{categoryId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int categoryId)
    {
        var organiser = await _db.Organisers.FirstOrDefaultAsync(o => o.UserId == CurrentUserId);
        if (organiser == null) return Forbid();

        var cat = await _db.Categories
            .Include(c => c.Event)
            .Include(c => c.Enrolments)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId
                                   && c.Event.OrganiserId == organiser.OrganiserId);
        if (cat == null) return NotFound();
        if (cat.Enrolments.Any()) return Conflict(new { message = "Category has enrolments." });

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}