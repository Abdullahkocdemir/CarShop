using DTOsLayer.WebUIDTO.TestimonialDTO; 
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
    public class TestimonialController : Controller 
    {
        private readonly HttpClient _httpClient;

        public TestimonialController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient"); 
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Testimonials"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultTestimonialDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultTestimonialDTO>()); 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create(CreateTestimonialDTO dto)
        {
            if (ModelState.IsValid) 
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Testimonials", content); 
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Referans başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Referans eklenirken bir hata oluştu.";
            }
            return View(dto); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Testimonials/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateTestimonialDTO>(jsonData); 
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans bulunamadı.";
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTestimonialDTO dto)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Testimonials", content); 
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Referans başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Referans güncellenirken bir hata oluştu.";
            }
            return View(dto); 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Testimonials/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Referans başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans silinirken bir hata oluştu.";
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Testimonials/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdTestimonialDTO>(jsonData); 
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}