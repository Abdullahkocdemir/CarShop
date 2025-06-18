using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.FeatureDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : BaseEntityController
    {
        private readonly IFeatureService _featureService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        protected override string EntityTypeName => "Feature";

        public FeaturesController(IFeatureService featureService, IMapper mapper,
                                  EnhancedRabbitMQService rabbitMqService,
                                  IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _featureService = featureService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetAbsoluteUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }

            if (Uri.IsWellFormedUriString(relativePath, UriKind.Absolute))
            {
                return relativePath;
            }

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            return $"{baseUrl.TrimEnd('/')}{relativePath}"; 
        }


        [HttpGet]
        public IActionResult GetListAllFeatures()
        {
            var features = _featureService.BGetListAll();
            var values = _mapper.Map<List<ResultFeatureDTO>>(features);

            foreach (var featureDto in values)
            {
                if (!string.IsNullOrEmpty(featureDto.ImageUrl))
                {
                    featureDto.ImageUrl = GetAbsoluteUrl(featureDto.ImageUrl);
                }
            }
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetFeatureById(int id)
        {
            var value = _featureService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID: {id} ile özellik bulunamadı.");
            }
            var dto = _mapper.Map<GetByIdFeatureDTO>(value);

            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                dto.ImageUrl = GetAbsoluteUrl(dto.ImageUrl);
            }

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeature([FromForm] CreateFeatureDTO dto)
        {
            var feature = _mapper.Map<Feature>(dto);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                feature.ImageUrl = "/images/" + uniqueFileName;
            }
            else
            {
                feature.ImageUrl = "";
            }

            _featureService.BAdd(feature);
            PublishEntityCreated(feature);

            var imageUrlForResponse = string.IsNullOrEmpty(feature.ImageUrl) ? "" : GetAbsoluteUrl(feature.ImageUrl);

            return Ok(new { Message = "Özellik başarıyla eklendi ve mesaj gönderildi.", FeatureId = feature.FeatureId, ImageUrl = imageUrlForResponse });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeature([FromForm] UpdateFeatureDTO dto, [FromForm] bool removeImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFeature = _featureService.BGetById(dto.FeatureId);
            if (existingFeature == null)
            {
                return NotFound($"Güncellenmek istenen özellik (ID: {dto.FeatureId}) bulunamadı.");
            }

            existingFeature.Title = dto.Title;
            existingFeature.SmallTitle = dto.SmallTitle;
            existingFeature.Description = dto.Description;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingFeature.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingFeature.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                existingFeature.ImageUrl = "/images/" + uniqueFileName;
            }
            else if (removeImage) 
            {
                if (!string.IsNullOrEmpty(existingFeature.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingFeature.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                existingFeature.ImageUrl = ""; 
            }

            _featureService.BUpdate(existingFeature);
            PublishEntityUpdated(existingFeature);

            var imageUrlForResponse = string.IsNullOrEmpty(existingFeature.ImageUrl) ? "" : GetAbsoluteUrl(existingFeature.ImageUrl);

            return Ok(new { Message = "Özellik başarıyla güncellendi ve mesaj yayınlandı.", FeatureId = existingFeature.FeatureId, ImageUrl = imageUrlForResponse });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFeature(int id)
        {
            var featureToDelete = _featureService.BGetById(id);
            if (featureToDelete == null)
            {
                return NotFound($"Silinmek istenen özellik (ID: {id}) bulunamadı.");
            }

            // Dosyayı sil
            if (!string.IsNullOrEmpty(featureToDelete.ImageUrl))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, featureToDelete.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _featureService.BDelete(featureToDelete);
            PublishEntityDeleted(featureToDelete);

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureId = id });
        }
    }
}