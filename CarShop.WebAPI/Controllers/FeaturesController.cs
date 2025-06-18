// CarShop.WebAPI.Controllers/FeaturesController.cs
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
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.TrimEnd('/');
            }
            return $"{baseUrl}{relativePath}";
        }

        private async Task<string> SaveImageFile(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
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
            return "/images/" + uniqueFileName;
        }

        private void DeleteImageFile(string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }



        [HttpGet]
        public IActionResult GetListAllFeatures()
        {
            var features = _featureService.BGetListWithImage();
            var values = _mapper.Map<List<ResultFeatureDTO>>(features);

            foreach (var featureDto in values)
            {
                if (featureDto.FeatureImages != null)
                {
                    foreach (var imageDto in featureDto.FeatureImages)
                    {
                        imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl);
                    }
                }
            }
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetFeatureById(int id)
        {
            var value = _featureService.BGetByIdWithImage(id);
            if (value == null)
            {
                return NotFound($"ID: {id} ile özellik bulunamadı.");
            }
            var dto = _mapper.Map<GetByIdFeatureDTO>(value);

            if (dto.FeatureImages != null)
            {
                foreach (var imageDto in dto.FeatureImages)
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl);
                }
            }
            return Ok(dto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateFeature([FromForm] CreateFeatureDTO dto)
        {
            var feature = _mapper.Map<Feature>(dto);
            _featureService.BAdd(feature);

            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                foreach (var imageFile in dto.ImageFiles)
                {
                    var imageUrl = await SaveImageFile(imageFile);
                    var featureImage = new FeatureImage
                    {
                        FeatureId = feature.FeatureId,
                        ImageUrl = imageUrl,
                        FileName = imageFile.FileName,
                        UploadDate = DateTime.UtcNow
                    };
                    _featureImageService.BAdd(featureImage);
                }
            }

            var featureWithImages = _featureService.BGetByIdWithImage(feature.FeatureId);

            var featureForRabbitMQ = new Feature
            {
                FeatureId = featureWithImages.FeatureId,
                Title = featureWithImages.Title,
                SmallTitle = featureWithImages.SmallTitle,
                Description = featureWithImages.Description
            };
            PublishEntityCreated(featureForRabbitMQ);

            var createdFeatureDto = _mapper.Map<ResultFeatureDTO>(featureWithImages);

            if (createdFeatureDto.FeatureImages != null)
            {
                foreach (var imageDto in createdFeatureDto.FeatureImages)
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl);
                }
            }

            return Ok(new { Message = "Özellik başarıyla eklendi ve mesaj gönderildi.", Feature = createdFeatureDto });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateFeature([FromForm] UpdateFeatureDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFeature = _featureService.BGetByIdWithImage(dto.FeatureId);
            if (existingFeature == null)
            {
                return NotFound($"Güncellenmek istenen özellik (ID: {dto.FeatureId}) bulunamadı.");
            }

            existingFeature.Title = dto.Title;
            existingFeature.SmallTitle = dto.SmallTitle;
            existingFeature.Description = dto.Description;

            if (dto.ImageIdsToRemove != null && dto.ImageIdsToRemove.Any())
            {
                foreach (var imageId in dto.ImageIdsToRemove)
                {
                    var imageToDelete = _featureImageService.BGetById(imageId);
                    if (imageToDelete != null && imageToDelete.FeatureId == existingFeature.FeatureId)
                    {
                        DeleteImageFile(imageToDelete.ImageUrl);
                        _featureImageService.BDelete(imageToDelete);
                    }
                }
            }

            if (dto.NewImageFiles != null && dto.NewImageFiles.Any())
            {
                foreach (var newImageFile in dto.NewImageFiles)
                {
                    var newImageUrl = await SaveImageFile(newImageFile);
                    var newFeatureImage = new FeatureImage
                    {
                        FeatureId = existingFeature.FeatureId,
                        ImageUrl = newImageUrl,
                        FileName = newImageFile.FileName,
                        UploadDate = DateTime.UtcNow
                    };
                    _featureImageService.BAdd(newFeatureImage);
                }
            }

            _featureService.BUpdate(existingFeature);

            var updatedFeatureAfterSave = _featureService.BGetByIdWithImage(existingFeature.FeatureId);

            var updatedFeatureForRabbitMQ = new Feature
            {
                FeatureId = updatedFeatureAfterSave.FeatureId,
                Title = updatedFeatureAfterSave.Title,
                SmallTitle = updatedFeatureAfterSave.SmallTitle,
                Description = updatedFeatureAfterSave.Description
            };
            PublishEntityUpdated(updatedFeatureForRabbitMQ);

            var updatedFeatureDto = _mapper.Map<ResultFeatureDTO>(updatedFeatureAfterSave);

            if (updatedFeatureDto.FeatureImages != null)
            {
                foreach (var imageDto in updatedFeatureDto.FeatureImages)
                {
                    imageDto.ImageUrl = GetAbsoluteUrl(imageDto.ImageUrl);
                }
            }

            return Ok(new { Message = "Özellik başarıyla güncellendi ve mesaj yayınlandı.", Feature = updatedFeatureDto });
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteFeature(int id)
        {
            var featureToDelete = _featureService.BGetByIdWithImage(id);
            if (featureToDelete == null)
            {
                return NotFound($"Silinmek istenen özellik (ID: {id}) bulunamadı.");
            }
            if (featureToDelete.FeatureImages != null && featureToDelete.FeatureImages.Any())
            {
                foreach (var image in featureToDelete.FeatureImages.ToList())
                {
                    DeleteImageFile(image.ImageUrl);
                    _featureImageService.BDelete(image);
                }
            }

            _featureService.BDelete(featureToDelete);

            var deletedFeatureForRabbitMQ = new Feature
            {
                FeatureId = featureToDelete.FeatureId,
                Title = featureToDelete.Title,
                SmallTitle = featureToDelete.SmallTitle,
                Description = featureToDelete.Description
            };
            PublishEntityDeleted(deletedFeatureForRabbitMQ);

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureId = id });
        }


        [HttpDelete("image/{imageId}")]
        public IActionResult DeleteFeatureImage(int imageId)
        {
            var imageToDelete = _featureImageService.BGetById(imageId);
            if (imageToDelete == null)
            {
                return NotFound($"Silinmek istenen resim (ID: {imageId}) bulunamadı.");
            }

            DeleteImageFile(imageToDelete.ImageUrl);

            _featureImageService.BDelete(imageToDelete);

            return Ok(new { Message = $"Resim (ID: {imageId}) başarıyla silindi.", ImageId = imageId });
        }
    }
}