using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.PartnerDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : BaseEntityController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        protected override string EntityTypeName => "Partner";

        public PartnersController(IPartnerService partnerService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _partnerService = partnerService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Tüm iş ortağı öğelerini listeler.
        /// </summary>
        /// <returns>İş ortağı listesi.</returns>
        [HttpGet]
        public IActionResult GetListAllPartners()
        {
            var partners = _partnerService.BGetListAll();
            var partnerDtos = _mapper.Map<List<ResultPartnerDTO>>(partners);
            return Ok(partnerDtos);
        }

        /// <summary>
        /// Belirli bir ID'ye sahip iş ortağı öğesini getirir.
        /// </summary>
        /// <param name="id">İş ortağı ID'si.</param>
        /// <returns>Belirtilen ID'ye sahip iş ortağı.</returns>
        [HttpGet("{id}")]
        public IActionResult GetPartnerById(int id)
        {
            var partner = _partnerService.BGetById(id);
            if (partner == null)
            {
                return NotFound($"ID'si {id} olan iş ortağı bulunamadı.");
            }
            var partnerDto = _mapper.Map<GetByIdPartnerDTO>(partner);
            return Ok(partnerDto);
        }

        /// <summary>
        /// Yeni bir iş ortağı öğesi oluşturur. Resim dosyası Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Oluşturulacak iş ortağı verileri ve resim dosyası.</param>
        /// <returns>Başarılı sonuç mesajı ve oluşturulan iş ortağının bilgileri.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromForm] CreatePartnerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var partner = _mapper.Map<Partner>(dto);

            // Resim dosyası kontrolü ve kaydetme
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                partner.ImageUrl = await SaveImage(dto.ImageFile, "partners"); // "partners" klasörüne kaydet
            }
            else
            {
                return BadRequest("İş ortağı oluşturmak için bir resim dosyası gereklidir.");
            }

            _partnerService.BAdd(partner);
            PublishEntityCreated(partner);

            return StatusCode(201, new { Message = "İş ortağı başarıyla eklendi ve mesaj gönderildi.", PartnerId = partner.PartnerId, ImageUrl = partner.ImageUrl });
        }

        /// <summary>
        /// Mevcut bir iş ortağı öğesini günceller. Resim dosyası (isteğe bağlı) Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Güncellenecek iş ortağı verileri ve yeni resim dosyası (isteğe bağlı).</param>
        /// <returns>Başarılı sonuç mesajı ve güncellenen iş ortağının bilgileri.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromForm] UpdatePartnerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPartner = _partnerService.BGetById(dto.PartnerId);
            if (existingPartner == null)
            {
                return NotFound($"ID'si {dto.PartnerId} olan iş ortağı bulunamadı.");
            }

            _mapper.Map(dto, existingPartner);

            // Resim güncelleme mantığı
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Yeni resim yüklendiyse eski resmi sil
                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl, "partners");
                }
                // Yeni resmi kaydet ve URL'i güncelle
                existingPartner.ImageUrl = await SaveImage(dto.ImageFile, "partners");
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                // Yeni resim yüklenmedi ama mevcut bir URL varsa, onu koru
                existingPartner.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                // Ne yeni resim yüklendi ne de mevcut bir URL belirtildi.
                // Eski resim varsa sil ve ImageUrl'i boşalt.
                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl, "partners");
                }
                existingPartner.ImageUrl = string.Empty;
            }

            _partnerService.BUpdate(existingPartner);
            PublishEntityUpdated(existingPartner);

            return Ok(new { Message = "İş ortağı başarıyla güncellendi ve mesaj yayınlandı.", PartnerId = existingPartner.PartnerId, ImageUrl = existingPartner.ImageUrl });
        }

        /// <summary>
        /// Belirli bir ID'ye sahip iş ortağı öğesini siler.
        /// </summary>
        /// <param name="id">Silinecek iş ortağı ID'si.</param>
        /// <returns>Başarılı sonuç mesajı.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeletePartner(int id)
        {
            var partnerToDelete = _partnerService.BGetById(id);
            if (partnerToDelete == null)
            {
                return NotFound($"ID'si {id} olan iş ortağı bulunamadı.");
            }

            // İlişkili resim dosyasını sil
            if (!string.IsNullOrEmpty(partnerToDelete.ImageUrl))
            {
                DeleteImage(partnerToDelete.ImageUrl, "partners");
            }

            _partnerService.BDelete(partnerToDelete);
            PublishEntityDeleted(partnerToDelete);

            return Ok(new { Message = "İş ortağı başarıyla silindi ve mesaj yayınlandı.", PartnerId = id });
        }

        /// <summary>
        /// Yüklenen resmi belirtilen klasöre kaydeder ve URL'sini döndürür.
        /// </summary>
        /// <param name="imageFile">Yüklenecek resim dosyası.</param>
        /// <param name="folderName">Resmin kaydedileceği wwwroot altındaki klasör adı (örn: "partners").</param>
        /// <returns>Kaydedilen resmin tam URL'si (örn: http://localhost:port/partners/unique_file_name.jpg).</returns>
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

            // Veritabanına kaydedilecek tam URL'yi oluştur
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }

        /// <summary>
        /// Belirtilen URL'deki resim dosyasını sunucudan siler.
        /// </summary>
        /// <param name="imageUrl">Silinecek resmin tam URL'si (örn: http://localhost:port/partners/unique_file_name.jpg).</param>
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
