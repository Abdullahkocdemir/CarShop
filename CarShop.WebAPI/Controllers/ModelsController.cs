using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ModelDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : BaseEntityController
    {
        private readonly IModelService _modelService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Model";

        public ModelsController(IModelService modelService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _modelService = modelService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllModels()
        {
            var values = _modelService.BGetListAll();
            var result = _mapper.Map<List<ResultModelDTO>>(values);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateModel(CreateModelDTO dto)
        {
            var model = _mapper.Map<Model>(dto);
            _modelService.BAdd(model);
            PublishEntityCreated(model);
            return Ok(new { Message = "Model başarıyla eklendi ve mesaj yayınlandı.", ModelId = model.ModelId });
        }
        [HttpPut]
        public IActionResult UpdateModel(UpdateModelDTO dto)
        {
            var existingModel = _modelService.BGetById(dto.ModelId);

            if (existingModel == null)
            {
                return NotFound($"Model with ID {dto.ModelId} bulunamadı.");
            }

            _mapper.Map(dto, existingModel);
            _modelService.BUpdate(existingModel);
            PublishEntityUpdated(existingModel);

            return Ok(new { Message = "Model güncellendi ve mesaj yayınlandı.", ModelId = existingModel.ModelId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteModel(int id)
        {
            var model = _modelService.BGetById(id);
            if (model == null)
            {
                return NotFound($"Model with ID {id} bulunamadı.");
            }

            _modelService.BDelete(model);
            PublishEntityDeleted(model);
            return Ok(new { Message = "Model silindi ve mesaj yayınlandı.", ModelId = id });
        }
        [HttpGet("{id}")]
        public IActionResult GetByIdModel(int id)
        {
            var value = _modelService.BGetById(id);
            if (value == null)
            {
                return NotFound($"Model with ID {id} bulunamadı.");
            }
            var result = _mapper.Map<GetByIdModelDTO>(value);
            return Ok(result);
        }
    }
}