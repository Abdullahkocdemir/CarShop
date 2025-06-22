using DTOsLayer.WebUIDTO.AboutItemDTO; 
using EntityLayer.Entities;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 
using System;
using System.Text;

namespace CarShop.WebUI.Controllers
{
    public class AboutItemController : Controller
    {
        private readonly HttpClient _httpClient;

        public AboutItemController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/AboutItems");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutItemDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultAboutItemDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAboutItemDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/AboutItems", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "About Item başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "About Item eklenirken bir hata oluştu.";
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/AboutItems/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateAboutItemDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan About Item bulunamadı.";
            return RedirectToAction("Index"); // Redirect if not found
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAboutItemDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/AboutItems", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "About Item başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "About Item güncellenirken bir hata oluştu.";
            }
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/AboutItems/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "About Item başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan About Item silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/AboutItems/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdAboutItemDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan About Item detayları bulunamadı.";
            return RedirectToAction("Index");
        }

    }
}