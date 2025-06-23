using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.FeatureSubstancesDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureSubstancesController : BaseEntityController
    {
        private readonly IFeatureSubstanceService _featureSubstanceService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        protected override string EntityTypeName => "FeatureSubstance";

        public FeatureSubstancesController(
            IFeatureSubstanceService featureSubstanceService,
            IMapper mapper,
            EnhancedRabbitMQService rabbitMqService,
            IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _featureSubstanceService = featureSubstanceService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllFeatureSubstances()
        {
            var featureSubstances = _featureSubstanceService.BGetListAll();
            var featureSubstanceDtos = _mapper.Map<List<ResultFeatureSubstancesDTO>>(featureSubstances);
            return Ok(featureSubstanceDtos);
        }
        [HttpGet("{id}")]
        public IActionResult GetFeatureSubstanceById(int id)
        {
            var featureSubstance = _featureSubstanceService.BGetById(id);
            if (featureSubstance == null)
            {
                return NotFound($"ID'si {id} olan özellik bulunamadı.");
            }
            var featureSubstanceDto = _mapper.Map<GetByIdFeatureSubstancesDTO>(featureSubstance);
            return Ok(featureSubstanceDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateFeatureSubstance([FromForm] CreateFeatureSubstancesDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var featureSubstance = _mapper.Map<FeatureSubstance>(dto);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                featureSubstance.ImageUrl = await SaveImage(dto.ImageFile, "featuresubstances");
            }
            else
            {
                return BadRequest("Özellik oluşturmak için bir resim dosyası gereklidir.");
            }

            _featureSubstanceService.BAdd(featureSubstance);
            PublishEntityCreated(featureSubstance);

            return StatusCode(201, new { Message = "Özellik başarıyla eklendi ve mesaj gönderildi.", FeatureSubstanceId = featureSubstance.FeatureSubstanceId, ImageUrl = featureSubstance.ImageUrl });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeatureSubstance([FromForm] UpdateFeatureSubstancesDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFeatureSubstance = _featureSubstanceService.BGetById(dto.FeatureSubstanceId);
            if (existingFeatureSubstance == null)
            {
                return NotFound($"ID'si {dto.FeatureSubstanceId} olan özellik bulunamadı.");
            }

            _mapper.Map(dto, existingFeatureSubstance);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingFeatureSubstance.ImageUrl))
                {
                    DeleteImage(existingFeatureSubstance.ImageUrl, "featuresubstances");
                }
                existingFeatureSubstance.ImageUrl = await SaveImage(dto.ImageFile, "featuresubstances");
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingFeatureSubstance.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                if (!string.IsNullOrEmpty(existingFeatureSubstance.ImageUrl))
                {
                    DeleteImage(existingFeatureSubstance.ImageUrl, "featuresubstances");
                }
                existingFeatureSubstance.ImageUrl = string.Empty;
            }

            _featureSubstanceService.BUpdate(existingFeatureSubstance);
            PublishEntityUpdated(existingFeatureSubstance);

            return Ok(new { Message = "Özellik başarıyla güncellendi ve mesaj yayınlandı.", FeatureSubstanceId = existingFeatureSubstance.FeatureSubstanceId, ImageUrl = existingFeatureSubstance.ImageUrl });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteFeatureSubstance(int id)
        {
            var featureSubstanceToDelete = _featureSubstanceService.BGetById(id);
            if (featureSubstanceToDelete == null)
            {
                return NotFound($"ID'si {id} olan özellik bulunamadı.");
            }

            if (!string.IsNullOrEmpty(featureSubstanceToDelete.ImageUrl))
            {
                DeleteImage(featureSubstanceToDelete.ImageUrl, "featuresubstances");
            }

            _featureSubstanceService.BDelete(featureSubstanceToDelete);
            PublishEntityDeleted(featureSubstanceToDelete);

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureSubstanceId = id });
        }
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

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }
        private void DeleteImage(string imageUrl, string folderName)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

