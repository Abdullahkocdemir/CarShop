using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTOsLayer.WebApiDTO.CalltoActionDTO;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalltoActionsController : BaseEntityController // Genellikle çoğul isim kullanılır, "Gotus" veya "Goes" gibi.
    {
        private readonly ICalltoActionService _calltoActionService; // IGotoService'i enjekte et
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "CalltoAction"; 

        public CalltoActionsController(ICalltoActionService calltoActionService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _calltoActionService = calltoActionService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Tüm Goto öğelerini listeler.
        /// </summary>
        [HttpGet]
        public IActionResult GetListAllGotus()
        {
            var gotus = _calltoActionService.BGetListAll();
            var gotoDtos = _mapper.Map<List<ResultCalltoActionDTO>>(gotus);
            return Ok(gotoDtos);
        }

        /// <summary>
        /// Belirli bir ID'ye sahip Goto öğesini getirir.
        /// </summary>
        /// <param name="id">Goto ID'si.</param>
        [HttpGet("{id}")]
        public IActionResult GetGotoById(int id)
        {
            var @goto = _calltoActionService.BGetById(id); // @goto, 'goto' anahtar kelimesiyle çakışmayı engeller
            if (@goto == null)
            {
                return NotFound($"ID'si {id} olan Goto bulunamadı.");
            }
            var gotoDto = _mapper.Map<GetByIdCalltoActionDTO>(@goto);
            return Ok(gotoDto);
        }

        /// <summary>
        /// Yeni bir Goto öğesi oluşturur. Resim dosyası Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Oluşturulacak Goto verileri ve resim dosyası.</param>
        [HttpPost]
        public async Task<IActionResult> CreateGoto([FromForm] CreateCalltoActionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @goto = _mapper.Map<CalltoAction>(dto);

            // Resim yükleme işlemi
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                @goto.ImageUrl = await SaveImage(dto.ImageFile, "goto"); // 'goto' klasörüne kaydet
            }
            else
            {
                // Resim dosyası zorunlu ise hata döndür
                return BadRequest("Resim dosyası yüklenmelidir.");
            }

            _calltoActionService.BAdd(@goto);
            PublishEntityCreated(@goto); // RabbitMQ'ya mesaj yayınla

            return StatusCode(201, new { Message = "Goto başarıyla eklendi ve mesaj gönderildi.", GotoId = @goto.CalltoActionId, ImageUrl = @goto.ImageUrl });
        }

        /// <summary>
        /// Mevcut bir Goto öğesini günceller. Resim dosyası (isteğe bağlı) Form-Data olarak gönderilmelidir.
        /// </summary>
        /// <param name="dto">Güncellenecek Goto verileri ve yeni resim dosyası (isteğe bağlı).</param>
        [HttpPut]
        public async Task<IActionResult> UpdateGoto([FromForm] UpdateCalltoActionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGoto = _calltoActionService.BGetById(dto.CalltoActionId);
            if (existingGoto == null)
            {
                return NotFound($"ID'si {dto.CalltoActionId} olan Goto bulunamadı.");
            }

            _mapper.Map(dto, existingGoto); // DTO'dan varlığa map'le

            // Yeni resim yüklenip yüklenmediğini kontrol et
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Eski resmi sil (eğer varsa ve yeni bir resim yükleniyorsa)
                if (!string.IsNullOrEmpty(existingGoto.ImageUrl))
                {
                    DeleteImage(existingGoto.ImageUrl, "goto");
                }
                existingGoto.ImageUrl = await SaveImage(dto.ImageFile, "goto"); // Yeni resmi kaydet
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                // Yeni dosya yüklenmediyse ancak mevcut bir URL varsa, onu koru
                existingGoto.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                // Ne yeni dosya ne de mevcut URL sağlanmadıysa, boş string olarak ayarla
                existingGoto.ImageUrl = string.Empty;
            }

            _calltoActionService.BUpdate(existingGoto);
            PublishEntityUpdated(existingGoto); // RabbitMQ'ya mesaj yayınla

            return Ok(new { Message = "Goto başarıyla güncellendi ve mesaj yayınlandı.", GotoId = existingGoto.CalltoActionId, ImageUrl = existingGoto.ImageUrl });
        }

        /// <summary>
        /// Belirli bir ID'ye sahip Goto öğesini siler.
        /// </summary>
        /// <param name="id">Silinecek Goto ID'si.</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteGoto(int id)
        {
            var gotoToDelete = _calltoActionService.BGetById(id);
            if (gotoToDelete == null)
            {
                return NotFound($"ID'si {id} olan Goto bulunamadı.");
            }

            // İlişkili resim dosyasını sil
            if (!string.IsNullOrEmpty(gotoToDelete.ImageUrl))
            {
                DeleteImage(gotoToDelete.ImageUrl, "goto");
            }

            _calltoActionService.BDelete(gotoToDelete);
            PublishEntityDeleted(gotoToDelete); // RabbitMQ'ya mesaj yayınla

            return Ok(new { Message = "Goto başarıyla silindi ve mesaj yayınlandı.", GotoId = id });
        }

        /// <summary>
        /// Yüklenen resmi belirtilen klasöre kaydeder ve URL'sini döndürür.
        /// </summary>
        /// <param name="imageFile">Yüklenecek resim dosyası.</param>
        /// <param name="folderName">Resmin kaydedileceği wwwroot altındaki klasör adı (örn: "goto").</param>
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