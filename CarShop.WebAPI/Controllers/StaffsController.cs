using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.StaffDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic; // List için eklendi
using System.Threading.Tasks; // async/await için eklendi

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : BaseEntityController
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "Staff";

        public StaffsController(IStaffService staffService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _staffService = staffService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Tüm personel öğelerini listeler.
        /// </summary>
        /// <returns>Personel listesi.</returns>
        [HttpGet]
        public IActionResult GetListAllStaffs()
        {
            var staffs = _staffService.BGetListAll();
            var staffDtos = _mapper.Map<List<ResultStaffDTO>>(staffs);
            return Ok(staffDtos);
        }

        /// <summary>
        /// Belirli bir ID'ye sahip personel öğesini getirir.
        /// </summary>
        /// <param name="id">Personel ID'si.</param>
        /// <returns>Belirtilen ID'ye sahip personel.</returns>
        [HttpGet("{id}")]
        public IActionResult GetStaffById(int id)
        {
            var staff = _staffService.BGetById(id);
            if (staff == null)
            {
                return NotFound($"ID'si {id} olan personel bulunamadı.");
            }
            var staffDto = _mapper.Map<GetByIdStaffDTO>(staff);
            return Ok(staffDto);
        }

        /// <summary>
        /// Yeni bir personel öğesi oluşturur. Resim dosyası Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Oluşturulacak personel verileri ve resim dosyası.</param>
        /// <returns>Başarılı sonuç mesajı ve oluşturulan personelin bilgileri.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromForm] CreateStaffDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staff = _mapper.Map<Staff>(dto);

            // Resim dosyası kontrolü ve kaydetme
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                staff.ImageUrl = await SaveImage(dto.ImageFile, "staffs"); // "staffs" klasörüne kaydet
            }
            else
            {
                return BadRequest("Personel oluşturmak için bir resim dosyası gereklidir.");
            }

            _staffService.BAdd(staff);
            PublishEntityCreated(staff);

            return StatusCode(201, new { Message = "Personel başarıyla eklendi ve mesaj gönderildi.", StaffId = staff.StaffId, ImageUrl = staff.ImageUrl });
        }

        /// <summary>
        /// Mevcut bir personel öğesini günceller. Resim dosyası (isteğe bağlı) Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Güncellenecek personel verileri ve yeni resim dosyası (isteğe bağlı).</param>
        /// <returns>Başarılı sonuç mesajı ve güncellenen personelin bilgileri.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateStaff([FromForm] UpdateStaffDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingStaff = _staffService.BGetById(dto.StaffId);
            if (existingStaff == null)
            {
                return NotFound($"ID'si {dto.StaffId} olan personel bulunamadı.");
            }

            _mapper.Map(dto, existingStaff);

            // Resim güncelleme mantığı
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Yeni resim yüklendiyse eski resmi sil
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl, "staffs");
                }
                // Yeni resmi kaydet ve URL'i güncelle
                existingStaff.ImageUrl = await SaveImage(dto.ImageFile, "staffs");
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                // Yeni resim yüklenmedi ama mevcut bir URL varsa, onu koru
                existingStaff.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                // Ne yeni resim yüklendi ne de mevcut bir URL belirtildi.
                // Eski resim varsa sil ve ImageUrl'i boşalt.
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl, "staffs");
                }
                existingStaff.ImageUrl = string.Empty;
            }

            _staffService.BUpdate(existingStaff);
            PublishEntityUpdated(existingStaff);

            return Ok(new { Message = "Personel başarıyla güncellendi ve mesaj yayınlandı.", StaffId = existingStaff.StaffId, ImageUrl = existingStaff.ImageUrl });
        }

        /// <summary>
        /// Belirli bir ID'ye sahip personel öğesini siler.
        /// </summary>
        /// <param name="id">Silinecek personel ID'si.</param>
        /// <returns>Başarılı sonuç mesajı.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteStaff(int id)
        {
            var staffToDelete = _staffService.BGetById(id);
            if (staffToDelete == null)
            {
                return NotFound($"ID'si {id} olan personel bulunamadı.");
            }

            // İlişkili resim dosyasını sil
            if (!string.IsNullOrEmpty(staffToDelete.ImageUrl))
            {
                DeleteImage(staffToDelete.ImageUrl, "staffs");
            }

            _staffService.BDelete(staffToDelete);
            PublishEntityDeleted(staffToDelete);

            return Ok(new { Message = "Personel başarıyla silindi ve mesaj yayınlandı.", StaffId = id });
        }

        /// <summary>
        /// Yüklenen resmi belirtilen klasöre kaydeder ve URL'sini döndürür.
        /// </summary>
        /// <param name="imageFile">Yüklenecek resim dosyası.</param>
        /// <param name="folderName">Resmin kaydedileceği wwwroot altındaki klasör adı (örn: "staffs").</param>
        /// <returns>Kaydedilen resmin tam URL'si (örn: http://localhost:port/staffs/unique_file_name.jpg).</returns>
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
        /// <param name="imageUrl">Silinecek resmin tam URL'si (örn: http://localhost:port/staffs/unique_file_name.jpg).</param>
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
