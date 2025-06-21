using DTOsLayer.WebUIDTO.CallBackDTO;
using EntityLayer.Entities;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CarShop.WebUI.Controllers
{
    public class CallBackController : Controller
    {
        private readonly HttpClient _httpClient;

        public CallBackController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CallBacks");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCallBackDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCallBackDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCallBackDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/CallBacks", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri arama isteği başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Geri arama isteği eklenirken bir hata oluştu.";
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBacks/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateCallBackDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCallBackDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/CallBacks", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri arama isteği başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Geri arama isteği güncellenirken bir hata oluştu.";
            }
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/CallBacks/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Geri arama isteği başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBacks/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCallBackDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}