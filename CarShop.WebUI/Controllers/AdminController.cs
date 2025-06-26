using BusinessLayer.RabbitMQ;
using Microsoft.AspNetCore.Mvc;
using System; 
using System.Linq; 
using System.Collections.Generic;
using CarShop.WebUI.Models;
using Newtonsoft.Json; 
using System.Text;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Threading.Tasks;
using System.Text.Json.Serialization; 

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
                var jsonString = await response.Content.ReadAsStringAsync();

                var serializerOptions = new System.Text.Json.JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    PropertyNameCaseInsensitive = true
                };

                try
                {
                    var users = System.Text.Json.JsonSerializer.Deserialize<List<UserListViewModel>>(jsonString, serializerOptions);
                    return View(users);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                    Console.WriteLine($"API'den Gelen Ham JSON: {jsonString}");
                    ViewData["ErrorMessage"] = "API'den gelen veri işlenemedi. Lütfen sistem yöneticisi ile iletişime geçin.";
                    return View(new List<UserListViewModel>());
                }
            }

            ViewData["ErrorMessage"] = $"API'ye erişim sırasında bir hata oluştu. Statü Kodu: {response.StatusCode}";
            return View(new List<UserListViewModel>());
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string id)
        {
            var client = _httpClientFactory.CreateClient("CarShopApiClient");

            var userResponse = await client.GetAsync($"api/Account/users/{id}");
            if (!userResponse.IsSuccessStatusCode)
            {
                if (userResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound($"Kullanıcı bulunamadı: {id}");
                }
                ViewData["ErrorMessage"] = $"Kullanıcı bilgileri alınırken bir hata oluştu. Statü Kodu: {userResponse.StatusCode}";
                return View("Error");
            }
            var jsonString = await userResponse.Content.ReadAsStringAsync();
            var serializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                PropertyNameCaseInsensitive = true
            };

            UserDetailDto userDetail;
            try
            {

                userDetail = System.Text.Json.JsonSerializer.Deserialize<UserDetailDto>(jsonString, serializerOptions)!;
                if (userDetail == null)
                {
                    return NotFound("Kullanıcı verisi deserialize edilemedi.");
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                Console.WriteLine($"API'den Gelen Ham JSON: {jsonString}");
                ViewData["ErrorMessage"] = "API'den gelen kullanıcı verisi işlenemedi.";
                return View("Error");
            }


            var rolesResponse = await client.GetAsync("api/roles");
            if (!rolesResponse.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = $"Roller alınırken bir hata oluştu. Statü Kodu: {rolesResponse.StatusCode}";
                return View("Error");
            }
            var allRoles = await rolesResponse.Content.ReadFromJsonAsync<List<RoleListDto>>(serializerOptions);


            var model = new ManageUserRolesViewModel
            {
                UserId = userDetail.Id!,
                UserName = userDetail.UserName!
            };

            foreach (var role in allRoles!)
            {
                model.Roles.Add(new RoleCheckBox
                {
                    RoleId = role.Id!,
                    RoleName = role.Name!,
                    IsSelected = userDetail.Roles!.Contains(role.Name!)
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

                var content = new StringContent(JsonConvert.SerializeObject(assignRoleDto), Encoding.UTF8, "application/json");

                if (role.IsSelected)
                {
                    await client.PostAsync("api/roles/assign", content);
                }
                else
                {
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