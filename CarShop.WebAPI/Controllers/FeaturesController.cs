using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.FeatureDTO; // Yeni oluşturacağımız Feature DTO'ları
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : BaseEntityController 
    {
        private readonly IFeatureService _featureService; 
        private readonly IMapper _mapper;

        protected override string EntityTypeName => "Feature";

        public FeaturesController(
            IFeatureService featureService,
            EnhancedRabbitMQService rabbitMqService,
            IMapper mapper)
            : base(rabbitMqService) 
        {
            _featureService = featureService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListFeature()
        {
            var features = _featureService.BGetListAll();
            var featureDtos = _mapper.Map<List<ResultFeatureDTO>>(features);
            return Ok(featureDtos);
        }
        [HttpGet("{id}")]
        public IActionResult GetByIdFeature(int id)
        {
            var feature = _featureService.BGetById(id);
            if (feature == null)
            {
                return NotFound($"ID'si {id} olan özellik bulunamadı.");
            }
            var featureDto = _mapper.Map<GetByIdFeatureDTO>(feature);
            return Ok(featureDto);
        }
        [HttpPost]
        public IActionResult CreateFeature(CreateFeatureDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var feature = _mapper.Map<Feature>(dto);
            _featureService.BAdd(feature);
            PublishEntityCreated(feature);

            return StatusCode(201, new { Message = "Özellik başarıyla eklendi ve mesaj yayınlandı.", FeatureId = feature.FeatureId });
        }
        [HttpPut]
        public IActionResult UpdateFeature(UpdateFeatureDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingFeature = _featureService.BGetById(dto.FeatureId);
            if (existingFeature == null)
            {
                return NotFound($"ID'si {dto.FeatureId} olan özellik bulunamadı.");
            }

            _mapper.Map(dto, existingFeature); 
            _featureService.BUpdate(existingFeature);
            PublishEntityUpdated(existingFeature);

            return Ok(new { Message = "Özellik başarıyla güncellendi ve mesaj yayınlandı.", FeatureId = existingFeature.FeatureId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteFeature(int id)
        {
            var featureToDelete = _featureService.BGetById(id);
            if (featureToDelete == null)
            {
                return NotFound($"ID'si {id} olan özellik bulunamadı.");
            }

            _featureService.BDelete(featureToDelete);
            PublishEntityDeleted(featureToDelete); 

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureId = id });
        }
    }
}