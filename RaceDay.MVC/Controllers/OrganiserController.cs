using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IO;
using System.Text;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiClient _api;

        public OrganiserController(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var client = _api.GetClient();
            var response = await client.GetAsync("api/v1/events");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonSerializer.Deserialize<PagedResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(pagedResult?.Items ?? new List<EventDto>());
            }

            return View(new List<EventDto>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto, IFormFile imageFile)
        {
            var client = _api.GetClient();
            
            // Ensure Status is set to prevent API validation errors
            if (string.IsNullOrEmpty(dto.Status)) dto.Status = "Open";

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/v1/events", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var createdEvent = JsonSerializer.Deserialize<EventDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (createdEvent != null && imageFile != null && imageFile.Length > 0)
                {
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StreamContent(imageFile.OpenReadStream()), "file", imageFile.FileName);
                    await client.PostAsync($"api/v1/events/{createdEvent.EventId}/upload-banner", formData);
                }

                return RedirectToAction(nameof(Index));
            }

            // THIS IS THE FIX: If the API fails, show the error on the screen
            var apiError = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"API Error ({(int)response.StatusCode}): {apiError}";
            
            return View(dto);
        }
    }
}