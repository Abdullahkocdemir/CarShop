using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.AboutFeature;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using System.Threading.Tasks; 
using System.Collections.Generic; 

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutFeaturesController : BaseEntityController
    {
        private readonly IAboutFeatureService _aboutFeatureService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment; 

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

        [HttpGet]
        public IActionResult GetListAboutFeature()
        {
            var values = _aboutFeatureService.BGetListAll();
            var resultDto = _mapper.Map<List<ResultAboutFeatureDTO>>(values);
            return Ok(resultDto);
        }

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

        [HttpPost]
        public async Task<IActionResult> CreateAboutFeature([FromForm] CreateAboutFeatureDTO createAboutFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aboutFeature = _mapper.Map<AboutFeature>(createAboutFeatureDto);

            if (createAboutFeatureDto.ImageFile != null && createAboutFeatureDto.ImageFile.Length > 0)
            {
                aboutFeature.ImageUrl = await SaveImage(createAboutFeatureDto.ImageFile, "Aboutfeature");
            }
            else
            {
                return BadRequest("Resim dosyası yüklenmelidir.");
            }

            _aboutFeatureService.BAdd(aboutFeature);
            PublishEntityCreated(aboutFeature);

            return StatusCode(201, new { Message = "AboutFeature başarıyla eklendi ve mesaj gönderildi.", AboutFeatureId = aboutFeature.AboutFeatureId, ImageUrl = aboutFeature.ImageUrl });
        }

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

            _mapper.Map(updateAboutFeatureDto, existingAboutFeature);

            if (updateAboutFeatureDto.ImageFile != null && updateAboutFeatureDto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingAboutFeature.ImageUrl))
                {
                    DeleteImage(existingAboutFeature.ImageUrl, "Aboutfeature");
                }
                existingAboutFeature.ImageUrl = await SaveImage(updateAboutFeatureDto.ImageFile, "Aboutfeature");
            }
            else if (!string.IsNullOrEmpty(updateAboutFeatureDto.ExistingImageUrl))
            {
                existingAboutFeature.ImageUrl = updateAboutFeatureDto.ExistingImageUrl;
            }

            _aboutFeatureService.BUpdate(existingAboutFeature);
            PublishEntityUpdated(existingAboutFeature);

            return Ok(new { Message = "AboutFeature başarıyla güncellendi ve mesaj yayınlandı.", AboutFeatureId = existingAboutFeature.AboutFeatureId, ImageUrl = existingAboutFeature.ImageUrl });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAboutFeature(int id)
        {
            var value = _aboutFeatureService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan AboutFeature bulunamadı.");
            }

            if (!string.IsNullOrEmpty(value.ImageUrl))
            {
                DeleteImage(value.ImageUrl, "Aboutfeature");
            }

            _aboutFeatureService.BDelete(value);
            PublishEntityDeleted(value);

            return Ok(new { Message = "AboutFeature başarıyla silindi ve mesaj yayınlandı.", AboutFeatureId = id });
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
