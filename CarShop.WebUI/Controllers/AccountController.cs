using CarShop.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;

namespace CarShop.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var jsonData = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/account/register", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin." + errorContent);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/account/login", content);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonNode.Parse(jsonString);
                var token = jsonObject!["token"]!.ToString();

                // JWT token'dan kullanıcı bilgilerini çıkar
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                // Kullanıcı bilgilerini ViewData'ya ekle (isteğe bağlı)
                ViewData["UserName"] = jsonToken.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value ??
                                      jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value?.Split('@')[0] ?? "Kullanıcı";
                ViewData["UserRole"] = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? "Kullanıcı";

                HttpContext.Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = false, // JavaScript'ten erişim için false yapıldı
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(3)
                });

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwtToken");
            return RedirectToAction("Index", "AdminProduct");
        }

        // Kullanıcı bilgilerini almak için API endpoint'i
        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var token = Request.Cookies["jwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Token bulunamadı" });
                }

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var firstName = jsonToken.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value ?? "";
                var lastName = jsonToken.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value ?? "";
                var email = jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "";
                var role = jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? "Kullanıcı";

                var fullName = (firstName + " " + lastName).Trim();
                if (string.IsNullOrEmpty(fullName))
                {
                    fullName = email.Split('@')[0];
                }

                return Json(new
                {
                    success = true,
                    userName = fullName,
                    userRole = role,
                    email = email
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Token çözümlenemedi: " + ex.Message });
            }
        }
    }
}