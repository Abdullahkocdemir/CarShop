using DTOsLayer.WebUIDTO.BlogDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminBlogController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateBlogDTO> _createBlogValidator;
        private readonly IValidator<UpdateBlogDTO> _updateBlogValidator;

        public AdminBlogController(IHttpClientFactory httpClientFactory,
                                 IValidator<CreateBlogDTO> createBlogValidator,
                                 IValidator<UpdateBlogDTO> updateBlogValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createBlogValidator = createBlogValidator;
            _updateBlogValidator = updateBlogValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Blogs");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBlogDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Bloglar yüklenirken bir hata oluştu.");
            return View(new List<ResultBlogDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogDTO dto)
        {
            ValidationResult result = await _createBlogValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");
                formData.Add(new StringContent(dto.Author), "Author");
                formData.Add(new StringContent(dto.SmallDescription), "SmallDescription");
                // Tarih formatını burada düzgünce stringe çeviriyoruz
                formData.Add(new StringContent(dto.Date.ToString("yyyy-MM-ddTHH:mm:ss")), "Date");
                formData.Add(new StringContent(dto.CommentCount.ToString()), "CommentCount");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");
                formData.Add(new StringContent(dto.PopulerBlog.ToString()), "PopulerBlog");

                if (dto.BannerImage != null)
                {
                    var fileContent = new StreamContent(dto.BannerImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.BannerImage.ContentType);
                    formData.Add(fileContent, "BannerImage", dto.BannerImage.FileName);
                }

                if (dto.MainImage != null)
                {
                    var fileContent = new StreamContent(dto.MainImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.MainImage.ContentType);
                    formData.Add(fileContent, "MainImage", dto.MainImage.FileName);
                }

                var response = await _httpClient.PostAsync("api/Blogs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Blog başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Blogs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiBlogDto = JsonConvert.DeserializeObject<GetByIdBlogDTO>(jsonData);

                var updateDto = new UpdateBlogDTO
                {
                    BlogId = apiBlogDto!.BlogId,
                    SmallTitle = apiBlogDto.SmallTitle,
                    Author = apiBlogDto.Author,
                    SmallDescription = apiBlogDto.SmallDescription,
                    Date = apiBlogDto.Date, // API'den gelen DateTime'ı olduğu gibi kullanıyoruz
                    CommentCount = apiBlogDto.CommentCount,
                    Title = apiBlogDto.Title,
                    Description = apiBlogDto.Description,
                    PopulerBlog = apiBlogDto.PopulerBlog,
                    ExistingBannerImageUrl = apiBlogDto.BannerImageUrl,
                    ExistingMainImageUrl = apiBlogDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan blog bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBlogDTO dto)
        {
            ValidationResult result = await _updateBlogValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.BlogId.ToString()), "BlogId");
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");
                formData.Add(new StringContent(dto.Author), "Author");
                formData.Add(new StringContent(dto.SmallDescription), "SmallDescription");
                // Tarih formatını burada düzgünce stringe çeviriyoruz
                formData.Add(new StringContent(dto.Date.ToString("yyyy-MM-ddTHH:mm:ss")), "Date");
                formData.Add(new StringContent(dto.CommentCount.ToString()), "CommentCount");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");
                formData.Add(new StringContent(dto.PopulerBlog.ToString()), "PopulerBlog");

                if (!string.IsNullOrEmpty(dto.ExistingBannerImageUrl))
                {
                    // ExistingImageUrl'ları API tarafı bekliyorsa ekleyin. Genelde dosya güncellemede bu bilgiye API'nin ihtiyacı olmaz.
                    // Çünkü yeni dosya varsa eskiyi siler, yoksa eskisi kalır.
                    // Ancak, API'nizdeki UpdateBlogDTO'da bu alanı kullanıyorsanız kalabilir.
                    formData.Add(new StringContent(dto.ExistingBannerImageUrl), "ExistingBannerImageUrl");
                }
                if (!string.IsNullOrEmpty(dto.ExistingMainImageUrl))
                {
                    formData.Add(new StringContent(dto.ExistingMainImageUrl), "ExistingMainImageUrl");
                }

                if (dto.BannerImage != null)
                {
                    var fileContent = new StreamContent(dto.BannerImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.BannerImage.ContentType);
                    formData.Add(fileContent, "BannerImage", dto.BannerImage.FileName);
                }

                if (dto.MainImage != null)
                {
                    var fileContent = new StreamContent(dto.MainImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.MainImage.ContentType);
                    formData.Add(fileContent, "MainImage", dto.MainImage.FileName);
                }

                var response = await _httpClient.PutAsync("api/Blogs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Blog başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Blogs/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Blog başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan blog silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Blogs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdBlogDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan blog detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}