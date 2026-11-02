using RaceDay.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using RaceDay.MVC.Services;
using System.Text;
using System.Text.Json;

[Authorize(Roles = "Participant")]
public class ParticipantController : Controller
{
    private readonly ApiClient _api;
    public ParticipantController(ApiClient api) => _api = api;

    [AllowAnonymous] // Home page is public
    public async Task<IActionResult> Index()
    {
        var client = _api.GetClient();
        var response = await client.GetAsync("api/v1/events");
        var events = JsonSerializer.Deserialize<List<EventDto>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(events);
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        var client = _api.GetClient();
        var payload = new { CategoryId = categoryId };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"api/v1/events/{eventId}/enrol", content);

        if (response.IsSuccessStatusCode) return RedirectToAction("MyResults");

        TempData["Error"] = "Could not enrol. You may already be registered.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> MyResults()
    {
        var client = _api.GetClient();
        // Call the participant history endpoint (ensure you implement this in your API!)
        var response = await client.GetAsync("api/v1/participants/history");
        var history = JsonSerializer.Deserialize<List<EnrolmentDto>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(history);
    }
}