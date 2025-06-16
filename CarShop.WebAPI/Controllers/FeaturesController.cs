using AutoMapper;
using BusinessLayer.Abstract; // IFeatureService için
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.FeatureDTO; // Yeni DTO'larınız
using EntityLayer.Entities; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : BaseEntityController
    {
        private readonly IFeatureService _featureService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Feature"; 

        public FeaturesController(IFeatureService featureService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _featureService = featureService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllFeatures() 
        {
            var values = _mapper.Map<List<ResultFeatureDTO>>(_featureService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetFeatureById(int id) 
        {
            var value = _featureService.BGetById(id);
            return Ok(value);
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

            return Ok(new { Message = "Özellik başarıyla eklendi ve mesaj gönderildi.", FeatureId = feature.FeatureId });
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
                return NotFound($"Güncellenmek istenen özellik (ID: {dto.FeatureId}) bulunamadı.");
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

            _featureService.BDelete(featureToDelete);
            PublishEntityDeleted(featureToDelete);

            return Ok(new { Message = "Özellik başarıyla silindi ve mesaj yayınlandı.", FeatureId = id });
        }
    }
}