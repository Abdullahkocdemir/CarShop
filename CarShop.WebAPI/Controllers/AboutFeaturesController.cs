using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.AboutFeature;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; // IWebHostEnvironment için
using System.IO; // Dosya işlemleri için
using System.Threading.Tasks; // async operasyonlar için
using System.Collections.Generic; // List için

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutFeaturesController : BaseEntityController
    {
        private readonly IAboutFeatureService _aboutFeatureService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment; // wwwroot yolunu almak için

        protected override string EntityTypeName => "AboutFeature";

        public AboutFeaturesController(
            IAboutFeatureService aboutFeatureService,
            EnhancedRabbitMQService rabbitMqService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _aboutFeatureService = aboutFeatureService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/AboutFeatures
        [HttpGet]
        public IActionResult GetListAboutFeature()
        {
            var values = _aboutFeatureService.BGetListAll();
            var resultDto = _mapper.Map<List<ResultAboutFeatureDTO>>(values);
            return Ok(resultDto);
        }

        // GET: api/AboutFeatures/{id}
        [HttpGet("{id}")]
        public IActionResult GetByIdAboutFeature(int id)
        {
            var value = _aboutFeatureService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan AboutFeature bulunamadı.");
            }
            var resultDto = _mapper.Map<GetByIdAboutFeatureDTO>(value);
            return Ok(resultDto);
        }

        // POST: api/AboutFeatures
        [HttpPost]
        public async Task<IActionResult> CreateAboutFeature([FromForm] CreateAboutFeatureDTO createAboutFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aboutFeature = _mapper.Map<AboutFeature>(createAboutFeatureDto);

            // Resim yükleme işlemi varsa
            if (createAboutFeatureDto.ImageFile != null && createAboutFeatureDto.ImageFile.Length > 0)
            {
                aboutFeature.ImageUrl = await SaveImage(createAboutFeatureDto.ImageFile, "Aboutfeature");
            }
            else
            {
                // Resim dosyası zorunlu ise BadRequest döndür.
                // Eğer resim zorunlu değilse bu satırı kaldırabilirsiniz.
                return BadRequest("Resim dosyası yüklenmelidir.");
            }

            _aboutFeatureService.BAdd(aboutFeature);
            // RabbitMQ mesajını yayımla
            PublishEntityCreated(aboutFeature);

            return StatusCode(201, new { Message = "AboutFeature başarıyla eklendi ve mesaj gönderildi.", AboutFeatureId = aboutFeature.AboutFeatureId, ImageUrl = aboutFeature.ImageUrl });
        }

        // PUT: api/AboutFeatures
        [HttpPut]
        public async Task<IActionResult> UpdateAboutFeature([FromForm] UpdateAboutFeatureDTO updateAboutFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAboutFeature = _aboutFeatureService.BGetById(updateAboutFeatureDto.AboutFeatureId);
            if (existingAboutFeature == null)
            {
                return NotFound($"ID'si {updateAboutFeatureDto.AboutFeatureId} olan AboutFeature bulunamadı.");
            }

            // DTO'dan entity'ye map'le (ImageUrl hariç diğer alanlar için)
            _mapper.Map(updateAboutFeatureDto, existingAboutFeature);

            // Yeni resim yüklenip yüklenmediğini kontrol et
            if (updateAboutFeatureDto.ImageFile != null && updateAboutFeatureDto.ImageFile.Length > 0)
            {
                // Eski resmi sil (eğer varsa)
                if (!string.IsNullOrEmpty(existingAboutFeature.ImageUrl))
                {
                    DeleteImage(existingAboutFeature.ImageUrl, "Aboutfeature");
                }
                // Yeni resmi kaydet ve URL'yi güncelle
                existingAboutFeature.ImageUrl = await SaveImage(updateAboutFeatureDto.ImageFile, "Aboutfeature");
            }
            else if (!string.IsNullOrEmpty(updateAboutFeatureDto.ExistingImageUrl))
            {
                // Yeni dosya yüklenmediyse ve ExistingImageUrl sağlandıysa, mevcut URL'yi koru
                existingAboutFeature.ImageUrl = updateAboutFeatureDto.ExistingImageUrl;
            }
            // Eğer ne yeni dosya ne de ExistingImageUrl varsa, ImageUrl'ın nasıl davranması gerektiğine karar verilmeli.
            // Şu anki durumda, eğer `_mapper.Map` ImageUrl'ı null veya boş olarak atıyorsa öyle kalır.
            // Eğer eski resmin kalması isteniyorsa, yukarıdaki 'else if' bloğu yeterlidir.

            _aboutFeatureService.BUpdate(existingAboutFeature);
            // RabbitMQ mesajını yayımla
            PublishEntityUpdated(existingAboutFeature);

            return Ok(new { Message = "AboutFeature başarıyla güncellendi ve mesaj yayınlandı.", AboutFeatureId = existingAboutFeature.AboutFeatureId, ImageUrl = existingAboutFeature.ImageUrl });
        }

        // DELETE: api/AboutFeatures/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAboutFeature(int id)
        {
            var value = _aboutFeatureService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan AboutFeature bulunamadı.");
            }

            // Resmi sunucudan sil
            if (!string.IsNullOrEmpty(value.ImageUrl))
            {
                DeleteImage(value.ImageUrl, "Aboutfeature");
            }

            _aboutFeatureService.BDelete(value);
            // RabbitMQ mesajını yayımla
            PublishEntityDeleted(value);

            return Ok(new { Message = "AboutFeature başarıyla silindi ve mesaj yayınlandı.", AboutFeatureId = id });
        }

        // --- Yardımcı metotlar resim işlemleri için ---

        /// <summary>
        /// Yüklenen resmi belirtilen klasöre kaydeder ve URL'sini döndürür.
        /// </summary>
        /// <param name="imageFile">Yüklenecek resim dosyası.</param>
        /// <param name="folderName">Resmin kaydedileceği wwwroot altındaki klasör adı (örn: "Aboutfeature", "services").</param>
        /// <returns>Kaydedilen resmin URL'si.</returns>
        private async Task<string> SaveImage(IFormFile imageFile, string folderName)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Veritabanına kaydedilecek URL'yi oluştur
            // Uygulamanızın çalıştığı şema ve host adını kullanarak dinamik URL oluşturulur.
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }

        /// <summary>
        /// Belirtilen URL'deki resim dosyasını sunucudan siler.
        /// </summary>
        /// <param name="imageUrl">Silinecek resmin URL'si.</param>
        /// <param name="folderName">Resmin bulunduğu wwwroot altındaki klasör adı.</param>
        private void DeleteImage(string imageUrl, string folderName)
        {
            // URL'den dosya adını çıkar
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
