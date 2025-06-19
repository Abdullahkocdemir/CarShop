using AutoMapper; 
using BusinessLayer.Abstract; 
using BusinessLayer.RabbitMQ; 
using DTOsLayer.WebApiDTO.FeatureDTO; 
using DTOsLayer.WebApiDTO.FeatureImageDTO; 
using EntityLayer.Entities; 
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using System;
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    public class FeaturesController : BaseEntityController 
    {
        private readonly IFeatureService _featureService; 
        private readonly IFeatureImageService _featureImageService; 
        private readonly IMapper _mapper; 
        private readonly IWebHostEnvironment _webHostEnvironment; 

        protected override string EntityTypeName => "Feature"; 

        public FeaturesController(IFeatureService featureService, 
                                  IFeatureImageService featureImageService,
                                  IMapper mapper,
                                  EnhancedRabbitMQService rabbitMqService, 
                                  IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService) 
        {
            _featureService = featureService;
            _featureImageService = featureImageService; 
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment; 
        }

        private string GetAbsoluteUrl(string relativePath) // Göreceli bir URL yolunu mutlak bir URL'ye dönüştüren yardımcı metot.
        {
            if (string.IsNullOrEmpty(relativePath)) // Yol boş veya null ise boş bir string döndürür.
            {
                return string.Empty;
            }

            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute)) // Eğer yol zaten mutlak bir URL ise, olduğu gibi döndürür.
            {
                return relativePath;
            }

            var request = HttpContext.Request; 
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            if (!relativePath.StartsWith("/")) // Göreceli yol "/" ile başlamıyorsa, başına "/" ekler.
            {
                relativePath = "/" + relativePath;
            }
            if (baseUrl.EndsWith("/")) // Temel URL "/" ile bitiyorsa, sondaki "/" işaretini kaldırır.
            {
                baseUrl = baseUrl.TrimEnd('/');
            }
            return $"{baseUrl}{relativePath}"; // Mutlak URL'yi döndürür.
        }

        private async Task<string> SaveImageFile(IFormFile imageFile) // Gelen bir resim dosyasını sunucuya kaydeden asenkron metot.
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images"); // Resimlerin yükleneceği "images" klasörünün tam yolunu belirler.
            if (!Directory.Exists(uploadsFolder)) // Klasör yoksa oluşturur.
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName; // Benzersiz bir dosya adı oluşturur.
            var filePath = Path.Combine(uploadsFolder, uniqueFileName); // Dosyanın sunucudaki tam yolunu oluşturur.

            using (var fileStream = new FileStream(filePath, FileMode.Create)) // Dosya akışı oluşturur ve dosyayı yazar.
            {
                await imageFile.CopyToAsync(fileStream); // Gelen resim dosyasını akışa kopyalar.
            }
            return "/images/" + uniqueFileName; // Kaydedilen resmin göreceli URL'sini döndürür.
        }

        private void DeleteImageFile(string relativePath) // Sunucudaki bir resim dosyasını göreceli yoluna göre silen metot.
        {
            if (!string.IsNullOrEmpty(relativePath)) // Yol boş veya null değilse.
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/')); // Dosyanın sunucudaki tam yolunu oluşturur.
                if (System.IO.File.Exists(filePath)) // Dosya varsa siler.
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }


        [HttpGet] // HTTP GET isteği için bir endpoint tanımlar.
        public IActionResult GetListAllFeatures() // Tüm özellikleri listeleyen aksiyon metodu.
        {
            var features = _featureService.BGetListWithImage(); // Özellikleri resimleriyle birlikte iş katmanından alır.
            var values = _mapper.Map<List<ResultFeatureDTO>>(features); // Entity nesnelerini DTO'lara eşler.

            foreach (var featureDto in values) // Her bir özellik DTO'su için.
            {
                if (featureDto.FeatureImages != null) // Özellik resimleri varsa.
                {
                    foreach (var imageDto in featureDto.FeatureImages) // Her bir resim DTO'su için.
                    {
                        imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl); // Resim URL'sini mutlak URL'ye dönüştürür.
                    }
                }
            }
            return Ok(values); // Dönüştürülmüş DTO listesini HTTP 200 OK ile döndürür.
        }

        [HttpGet("{id}")] // Belirli bir ID'ye sahip özelliği getiren HTTP GET endpoint'i.
        public IActionResult GetFeatureById(int id) // Belirli bir ID'ye sahip özelliği getiren aksiyon metodu.
        {
            var value = _featureService.BGetByIdWithImage(id); // Belirtilen ID'ye sahip özelliği resimleriyle birlikte alır.
            if (value == null) // Eğer özellik bulunamazsa.
            {
                return NotFound($"ID: {id} ile özellik bulunamadı."); // HTTP 404 Not Found döndürür.
            }
            var dto = _mapper.Map<GetByIdFeatureDTO>(value); // Entity'yi DTO'ya eşler.

            if (dto.FeatureImages != null) // Özellik resimleri varsa.
            {
                foreach (var imageDto in dto.FeatureImages) // Her bir resim DTO'su için.
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl); // Resim URL'sini mutlak URL'ye dönüştürür.
                }
            }
            return Ok(dto); // Dönüştürülmüş DTO'yu HTTP 200 OK ile döndürür.
        }


        [HttpPost] // HTTP POST isteği için bir endpoint tanımlar (yeni bir özellik oluşturmak için).
        public async Task<IActionResult> CreateFeature([FromForm] CreateFeatureDTO dto) // Yeni bir özellik oluşturan asenkron aksiyon metodu. [FromForm] ile form verisi olarak alındığını belirtir.
        {
            var feature = _mapper.Map<Feature>(dto); // Gelen DTO'yu Feature entity'sine eşler.
            _featureService.BAdd(feature); // Özelliği veritabanına ekler.

            if (dto.ImageFiles != null && dto.ImageFiles.Any()) // Eğer yeni resim dosyaları varsa.
            {
                foreach (var imageFile in dto.ImageFiles) // Her bir resim dosyası için.
                {
                    var imageUrl = await SaveImageFile(imageFile); // Resmi sunucuya kaydeder ve URL'sini alır.
                    var featureImage = new FeatureImage // Yeni bir FeatureImage nesnesi oluşturur.
                    {
                        FeatureId = feature.FeatureId, // İlişkili özelliğin ID'sini atar.
                        ImageUrl = imageUrl, // Kaydedilen resmin URL'sini atar.
                        FileName = imageFile.FileName, // Dosya adını atar.
                        UploadDate = DateTime.UtcNow // Yükleme tarihini atar.
                    };
                    _featureImageService.BAdd(featureImage); // Özellik resmini veritabanına ekler.
                }
            }

            var featureWithImages = _featureService.BGetByIdWithImage(feature.FeatureId); // Yeni oluşturulan özelliği resimleriyle birlikte tekrar getirir.

            var featureForRabbitMQ = new Feature // RabbitMQ'ya gönderilecek Feature nesnesini hazırlar (gereksiz detayları içermeden).
            {
                FeatureId = featureWithImages.FeatureId,
                Title = featureWithImages.Title,
                SmallTitle = featureWithImages.SmallTitle,
                Description = featureWithImages.Description
            };
            PublishEntityCreated(featureForRabbitMQ); // Özellik oluşturuldu olayını RabbitMQ'ya yayımlar.

            var createdFeatureDto = _mapper.Map<ResultFeatureDTO>(featureWithImages); // Oluşturulan özelliği DTO'ya eşler.

            if (createdFeatureDto.FeatureImages != null) // Oluşturulan özelliğin resimleri varsa.
            {
                foreach (var imageDto in createdFeatureDto.FeatureImages) // Her bir resim DTO'su için.
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl); // Resim URL'sini mutlak URL'ye dönüştürür.
                }
            }

            return Ok(new { Message = "Özellik başarıyla eklendi ve mesaj gönderildi.", Feature = createdFeatureDto }); // Başarılı yanıt ve oluşturulan özellik DTO'su ile birlikte mesaj döndürür.
        }


        [HttpPut] // HTTP PUT isteği için bir endpoint tanımlar (mevcut bir özelliği güncellemek için).
        public async Task<IActionResult> UpdateFeature([FromForm] UpdateFeatureDTO dto) // Mevcut bir özelliği güncelleyen asenkron aksiyon metodu. [FromForm] ile form verisi olarak alındığını belirtir.
        {
            if (!ModelState.IsValid) // Model doğrulaması başarısız olursa.
            {
                return BadRequest(ModelState); // HTTP 400 Bad Request ve doğrulama hatalarını döndürür.
            }

            var existingFeature = _featureService.BGetByIdWithImage(dto.FeatureId); // Güncellenecek mevcut özelliği ID'sine göre resimleriyle birlikte alır.
            if (existingFeature == null) // Eğer özellik bulunamazsa.
            {
                return NotFound($"Güncellenmek istenen özellik (ID: {dto.FeatureId}) bulunamadı."); // HTTP 404 Not Found döndürür.
            }

            existingFeature.Title = dto.Title; // Başlığı günceller.
            existingFeature.SmallTitle = dto.SmallTitle; // Küçük başlığı günceller.
            existingFeature.Description = dto.Description; // Açıklamayı günceller.

            if (dto.ImageIdsToRemove != null && dto.ImageIdsToRemove.Any()) // Kaldırılacak resim ID'leri varsa.
            {
                foreach (var imageId in dto.ImageIdsToRemove) // Her bir resim ID'si için.
                {
                    var imageToDelete = _featureImageService.BGetById(imageId); // Silinecek resmi ID'sine göre alır.
                    if (imageToDelete != null && imageToDelete.FeatureId == existingFeature.FeatureId) // Resim varsa ve güncellenen özelliğe aitse.
                    {
                        DeleteImageFile(imageToDelete.ImageUrl); // Resim dosyasını sunucudan siler.
                        _featureImageService.BDelete(imageToDelete); // Resim kaydını veritabanından siler.
                    }
                }
            }

            if (dto.NewImageFiles != null && dto.NewImageFiles.Any()) // Yeni resim dosyaları varsa.
            {
                foreach (var newImageFile in dto.NewImageFiles) // Her bir yeni resim dosyası için.
                {
                    var newImageUrl = await SaveImageFile(newImageFile); // Yeni resmi sunucuya kaydeder ve URL'sini alır.
                    var newFeatureImage = new FeatureImage // Yeni bir FeatureImage nesnesi oluşturur.
                    {
                        FeatureId = existingFeature.FeatureId, // İlişkili özelliğin ID'sini atar.
                        ImageUrl = newImageUrl, // Kaydedilen resmin URL'sini atar.
                        FileName = newImageFile.FileName, // Dosya adını atar.
                        UploadDate = DateTime.UtcNow // Yükleme tarihini atar.
                    };
                    _featureImageService.BAdd(newFeatureImage); // Yeni özellik resmini veritabanına ekler.
                }
            }

            _featureService.BUpdate(existingFeature); // Özelliği veritabanında günceller.

            var updatedFeatureAfterSave = _featureService.BGetByIdWithImage(existingFeature.FeatureId); // Güncellenmiş özelliği resimleriyle birlikte tekrar getirir.

            var updatedFeatureForRabbitMQ = new Feature // RabbitMQ'ya gönderilecek güncellenmiş Feature nesnesini hazırlar.
            {
                FeatureId = updatedFeatureAfterSave.FeatureId,
                Title = updatedFeatureAfterSave.Title,
                SmallTitle = updatedFeatureAfterSave.SmallTitle,
                Description = updatedFeatureAfterSave.Description
            };
            PublishEntityUpdated(updatedFeatureForRabbitMQ); // Özellik güncellendi olayını RabbitMQ'ya yayımlar.

            var updatedFeatureDto = _mapper.Map<ResultFeatureDTO>(updatedFeatureAfterSave); // Güncellenmiş özelliği DTO'ya eşler.

            if (updatedFeatureDto.FeatureImages != null) // Güncellenen özelliğin resimleri varsa.
            {
                foreach (var imageDto in updatedFeatureDto.FeatureImages) // Her bir resim DTO'su için.
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl); // Resim URL'sini mutlak URL'ye dönüştürür.
                }
            }

            return Ok(new { Message = "Özellik başarıyla güncellendi ve mesaj yayınlandı.", Feature = updatedFeatureDto }); // Başarılı yanıt ve güncellenen özellik DTO'su ile birlikte mesaj döndürür.
        }


        [HttpDelete("{id}")] // Belirli bir ID'ye sahip özelliği silmek için HTTP DELETE endpoint'i.
        public IActionResult DeleteFeature(int id) // Belirli bir ID'ye sahip özelliği silen aksiyon metodu.
        {
            var featureToDelete = _featureService.BGetByIdWithImage(id); // Silinecek özelliği ID'sine göre resimleriyle birlikte alır.
            if (featureToDelete == null) // Eğer özellik bulunamazsa.
            {
                return NotFound($"Silinmek istenen özellik (ID: {id}) bulunamadı."); // HTTP 404 Not Found döndürür.
            }
            if (featureToDelete.FeatureImages != null && featureToDelete.FeatureImages.Any()) // Özelliğin resimleri varsa.
            {
                foreach (var image in featureToDelete.FeatureImages.ToList()) // Her bir resim için. (ToList() kullanımı, koleksiyonu iterasyon sırasında değiştirmeyi mümkün kılar)
                {
                    DeleteImageFile(image.ImageUrl); // Resim dosyasını sunucudan siler.
                    _featureImageService.BDelete(image); // Resim kaydını veritabanından siler.
                }
            }

            _featureService.BDelete(featureToDelete); // Özelliği veritabanından siler.

            var deletedFeatureForRabbitMQ = new Feature // RabbitMQ'ya gönderilecek silinen Feature nesnesini hazırlar.
            {
                FeatureId = featureToDelete.FeatureId,
                Title = featureToDelete.Title,
                SmallTitle = featureToDelete.SmallTitle,
                Description = featureToDelete.Description
            };
            PublishEntityDeleted(deletedFeatureForRabbitMQ); // Özellik silindi olayını RabbitMQ'ya yayımlar.

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureId = id }); // Başarılı yanıt ve silinen özelliğin ID'si ile birlikte mesaj döndürür.
        }


        [HttpDelete("image/{imageId}")] // Belirli bir resim ID'sine sahip özelliği silmek için HTTP DELETE endpoint'i.
        public IActionResult DeleteFeatureImage(int imageId) // Belirli bir resim ID'sine sahip özelliği silen aksiyon metodu.
        {
            var imageToDelete = _featureImageService.BGetById(imageId); // Silinecek resmi ID'sine göre alır.
            if (imageToDelete == null) // Eğer resim bulunamazsa.
            {
                return NotFound($"Silinmek istenen resim (ID: {imageId}) bulunamadı."); // HTTP 404 Not Found döndürür.
            }

            DeleteImageFile(imageToDelete.ImageUrl); // Resim dosyasını sunucudan siler.

            _featureImageService.BDelete(imageToDelete); // Resim kaydını veritabanından siler.

            return Ok(new { Message = $"Resim (ID: {imageId}) başarıyla silindi.", ImageId = imageId }); // Başarılı yanıt ve silinen resmin ID'si ile birlikte mesaj döndürür.
        }
    }
}