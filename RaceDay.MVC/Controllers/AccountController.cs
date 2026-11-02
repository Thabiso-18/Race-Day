using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _api;
        public AccountController(ApiClient api) => _api = api;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var client = _api.GetClient();
            var payload = new { Email = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/v1/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Store JWT in Session for API calls
                HttpContext.Session.SetString("JWToken", result.Token);

                // Create MVC Cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.User.Email),
                    new Claim(ClaimTypes.Role, result.User.Role)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return result.User.Role == "Organiser"
                    ? RedirectToAction("Index", "Organiser")
                    : RedirectToAction("Index", "Participant");

            }

            ViewBag.Error = "Invalid login.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

    public class LoginResponse { public string Token { get; set; } = string.Empty; public UserDto User { get; set; } = new(); }
    public class UserDto { public string Email { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; }
}