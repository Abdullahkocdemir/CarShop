using BusinessLayer.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using System; // DateTime için eklendi
using System.Linq; // OrderByDescending, Select, Take için eklendi
using System.Collections.Generic;
using CarShop.WebUI.Models;
using Newtonsoft.Json; // Bu kütüphane kullanılıyor
using System.Text;
using System.Net.Http; // IHttpClientFactory ve StringContent için gerekli
using System.Net.Http.Json; // ReadFromJsonAsync için gerekli
using System.Threading.Tasks;
using System.Text.Json.Serialization; // async/await için gerekli

namespace CarShop.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EnhancedRabbitMQConsumerService _consumerService;

        public AdminController(IHttpClientFactory httpClientFactory, EnhancedRabbitMQConsumerService consumerService)
        {
            _httpClientFactory = httpClientFactory;
            _consumerService = consumerService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult RabbitMQMessages()
        {
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            return View(messageCounts);
        }
        [HttpGet]
        public JsonResult GetMessageCounts()
        {
            var messageCounts = _consumerService.GetAllEntityMessageCounts();
            return Json(messageCounts);
        }
        [HttpGet]
        public JsonResult GetAllMessages()
        {
            var allMessages = EnhancedRabbitMQConsumerService.AllMessages.ToList();

            var recentMessages = allMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .Select(m => new
                {
                    EntityType = m.EntityType,
                    Operation = m.Operation,
                    Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = m.ToString()
                })
                .ToList();

            return Json(recentMessages);
        }
        [HttpGet]
        public JsonResult GetEntityMessages(string entityType)
        {
            string lowerCaseEntityType = entityType.ToLower();

            if (EnhancedRabbitMQConsumerService.MessagesByEntity.TryGetValue(lowerCaseEntityType, out var messages))
            {
                var recentMessages = messages
                    .OrderByDescending(m => m.Timestamp)
                    .Take(20)
                    .Select(m => new
                    {
                        EntityType = m.EntityType,
                        Operation = m.Operation,
                        Timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(recentMessages);
            }

            return Json(new List<object>());
        }
        public async Task<IActionResult> UserList()
        {
            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var response = await client.GetAsync("api/account/users");

            if (response.IsSuccessStatusCode)
            {
                // 1. Adım: Yanıtı direkt nesneye çevirmek yerine önce string olarak oku.
                var jsonString = await response.Content.ReadAsStringAsync();

                // 2. Adım: API'nin kullandığı JSON ayarlarının aynısını burada da tanımla.
                var serializerOptions = new System.Text.Json.JsonSerializerOptions
                {
                    // Bu ayar, API'den gelen "$id" ve "$values" alanlarını anlamasını sağlar.
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,

                    // Bu ayar, API'den gelen "userName" gibi alanları C#'daki "UserName" ile eşleştirebilmesini sağlar.
                    PropertyNameCaseInsensitive = true
                };

                try
                {
                    // 3. Adım: String'i, doğru ayarlarla C# listesine çevir.
                    var users = System.Text.Json.JsonSerializer.Deserialize<List<UserListViewModel>>(jsonString, serializerOptions);
                    return View(users);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    // Eğer çevirme işleminde hala bir hata olursa, bu bize sorunun ne olduğunu söyler.
                    // Hatayı ve gelen JSON metnini konsola yazdırarak sorunu daha iyi anlayabiliriz.
                    Console.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                    Console.WriteLine($"API'den Gelen Ham JSON: {jsonString}");

                    // Kullanıcıya bir hata sayfası göster.
                    ViewData["ErrorMessage"] = "API'den gelen veri işlenemedi. Lütfen sistem yöneticisi ile iletişime geçin.";
                    return View(new List<UserListViewModel>());
                }
            }

            // Eğer API 401, 403, 404 gibi bir hata kodu dönerse bu kısım çalışır.
            ViewData["ErrorMessage"] = $"API'ye erişim sırasında bir hata oluştu. Statü Kodu: {response.StatusCode}";
            return View(new List<UserListViewModel>());
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string id)
        {
            var client = _httpClientFactory.CreateClient("CarShopApiClient");

            // --- 1. Adım: Kullanıcıyı Al ---
            var userResponse = await client.GetAsync($"api/Account/users/{id}");
            if (!userResponse.IsSuccessStatusCode)
            {
                // Kullanıcı bulunamazsa veya başka bir API hatası olursa
                if (userResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound($"Kullanıcı bulunamadı: {id}");
                }
                ViewData["ErrorMessage"] = $"Kullanıcı bilgileri alınırken bir hata oluştu. Statü Kodu: {userResponse.StatusCode}";
                return View("Error"); // Hata göstermek için bir Error view'ınız olduğunu varsayarak
            }

            // --- 2. Adım: API yanıtını özel seçeneklerle işle ---
            var jsonString = await userResponse.Content.ReadAsStringAsync();
            var serializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                // Bu ayar, API'den gelen "$id" ve "$values" alanlarını anlamasını sağlar.
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                // Bu ayar, API'den gelen "userName" gibi alanları C#'daki "UserName" ile eşleştirebilmesini sağlar.
                PropertyNameCaseInsensitive = true
            };

            UserDetailDto userDetail;
            try
            {
                // JSON string'ini doğru ayarlarla C# nesnesine çevir.
                userDetail = System.Text.Json.JsonSerializer.Deserialize<UserDetailDto>(jsonString, serializerOptions);
                if (userDetail == null)
                {
                    return NotFound("Kullanıcı verisi deserialize edilemedi.");
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Hata durumunu loglayıp kullanıcıya bilgi ver
                Console.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                Console.WriteLine($"API'den Gelen Ham JSON: {jsonString}");
                ViewData["ErrorMessage"] = "API'den gelen kullanıcı verisi işlenemedi.";
                return View("Error");
            }


            // --- 3. Adım: Sistemdeki tüm rolleri al ---
            var rolesResponse = await client.GetAsync("api/roles");
            if (!rolesResponse.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = $"Roller alınırken bir hata oluştu. Statü Kodu: {rolesResponse.StatusCode}";
                return View("Error");
            }
            // Rollerde muhtemelen referans döngüsü olmaz, bu yüzden direkt okunabilir.
            // Eğer rollerde de benzer bir hata alırsanız, yukarıdaki deserialization mantığını buraya da uygulayın.
            var allRoles = await rolesResponse.Content.ReadFromJsonAsync<List<RoleListDto>>(serializerOptions);


            // --- 4. Adım: ViewModel'i oluştur ---
            var model = new ManageUserRolesViewModel
            {
                UserId = userDetail.Id,
                UserName = userDetail.UserName
            };

            foreach (var role in allRoles)
            {
                model.Roles.Add(new RoleCheckBox
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = userDetail.Roles.Contains(role.Name)
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
        {
            var client = _httpClientFactory.CreateClient("CarShopApiClient");

            foreach (var role in model.Roles)
            {
                var assignRoleDto = new { UserId = model.UserId, RoleName = role.RoleName };

                // --- DÜZELTİLEN SATIR ---
                // 'JsonSerializer.Serialize' yerine 'JsonConvert.SerializeObject' kullanıldı.
                var content = new StringContent(JsonConvert.SerializeObject(assignRoleDto), Encoding.UTF8, "application/json");

                if (role.IsSelected)
                {
                    // Rol atanacaksa
                    await client.PostAsync("api/roles/assign", content);
                }
                else
                {
                    // Rol kaldırılacaksa
                    await client.PostAsync("api/roles/remove", content);
                }
            }

            return RedirectToAction("UserList");
        }


        [HttpGet]
        public async Task<IActionResult> RoleList()
        {
            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var response = await client.GetAsync("api/account/roles");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                var roles = System.Text.Json.JsonSerializer.Deserialize<List<RoleListViewModel>>(jsonString, options);
                return View(roles);
            }

            ViewData["ErrorMessage"] = "Roller listelenirken bir hata oluştu.";
            return View(new List<RoleListViewModel>());
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/account/roles/create", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Rol başarıyla oluşturuldu.";
                return RedirectToAction("RoleList");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Rol oluşturulamadı: {errorContent}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var client = _httpClientFactory.CreateClient("CarShopApiClient");
            var response = await client.DeleteAsync($"api/account/roles/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Rol başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rol silinirken bir hata oluştu.";
            }

            return RedirectToAction("RoleList");
        }
    }
}