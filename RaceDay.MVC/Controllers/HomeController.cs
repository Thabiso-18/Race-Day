using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _api;

        public HomeController(ApiClient api)
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
                
                // Read the paginated object
                var pagedResult = JsonSerializer.Deserialize<PagedResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Return just the list of events to the view
                return View(pagedResult?.Items ?? new List<EventDto>());
            }

            return View(new List<EventDto>());
        }
    }

    // Helper class to match the API's paginated response
    public class PagedResponse
    {
        public List<EventDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}