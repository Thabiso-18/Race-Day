using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly IConfiguration _config;

        public EventController(RaceDayDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                    BannerImageUrl = e.BannerImageUrl,
                    OrganiserName = e.Organiser.OrganisationName,
                    Categories = e.Categories.Select(c => new CategoryDto
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
        public async Task<IActionResult> GetById(int eventId)
        {
            var e = await _db.Events
                .Include(x => x.Organiser)
                .Include(x => x.Categories).ThenInclude(c => c.Enrolments)
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
                BannerImageUrl = e.BannerImageUrl,
                OrganiserName = e.Organiser.OrganisationName,
                Categories = e.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    DistanceKm = c.DistanceKm,
                    EntryFee = c.EntryFee,
                    MaxParticipants = c.MaxParticipants,
                    CurrentEnrolments = c.Enrolments.Count
                }).ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
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

            return StatusCode(201, new { ev.EventId, ev.Name });
        }

        [HttpPost("{eventId:int}/upload-banner")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UploadBanner(int eventId, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound();

            try
            {
                var connectionString = _config["AzureBlobStorage"];
                if (string.IsNullOrEmpty(connectionString)) return StatusCode(500, "Azure Storage not configured.");

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("event-images");
                
                await containerClient.CreateIfNotExistsAsync();

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                ev.BannerImageUrl = blobClient.Uri.ToString();
                await _db.SaveChangesAsync();

                return Ok(new { url = ev.BannerImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}