using DTOsLayer.WebUIDTO.CallBackTitleDTO;
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
    public class CallBackTitleController : Controller
    {
        private readonly HttpClient _httpClient;

        public CallBackTitleController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CallBackTitles");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCallBackTitleDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCallBackTitleDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCallBackTitleDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/CallBackTitles", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Geri Arama Başlığı eklenirken bir hata oluştu.";
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBackTitles/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateCallBackTitleDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı bulunamadı.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCallBackTitleDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/CallBackTitles", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Geri Arama Başlığı güncellenirken bir hata oluştu.";
            }
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/CallBackTitles/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBackTitles/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCallBackTitleDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}