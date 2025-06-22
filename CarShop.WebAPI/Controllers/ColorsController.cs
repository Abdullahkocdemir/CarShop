using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ColorDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : BaseEntityController
    {
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Color";

        public ColorsController(IColorService colorService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _colorService = colorService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListAllColors()
        {
            var values = _colorService.BGetListAll();
            var result = _mapper.Map<List<ResultColorDTO>>(values);
            return Ok(result);
        }
        [HttpPost]
        public IActionResult CreateColor(CreateColorDTO dto)
        {
            var color = _mapper.Map<Color>(dto);
            _colorService.BAdd(color);
            PublishEntityCreated(color); 
            return Ok(new { Message = "Renk başarıyla eklendi ve mesaj yayınlandı.", ColorId = color.ColorId });
        }
        [HttpPut]
        public IActionResult UpdateColor(UpdateColorDTO dto)
        {
            var existingColor = _colorService.BGetById(dto.ColorId);

            if (existingColor == null)
            {
                return NotFound($"Color with ID {dto.ColorId} bulunamadı.");
            }

            _mapper.Map(dto, existingColor);
            _colorService.BUpdate(existingColor);
            PublishEntityUpdated(existingColor); 

            return Ok(new { Message = "Renk güncellendi ve mesaj yayınlandı.", ColorId = existingColor.ColorId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteColor(int id)
        {
            var color = _colorService.BGetById(id);
            if (color == null)
            {
                return NotFound($"Color with ID {id} bulunamadı.");
            }

            _colorService.BDelete(color);
            PublishEntityDeleted(color); 
            return Ok(new { Message = "Renk silindi ve mesaj yayınlandı.", ColorId = id });
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdColor(int id)
        {
            var value = _colorService.BGetById(id);
            if (value == null)
            {
                return NotFound($"Color with ID {id} bulunamadı.");
            }
            var result = _mapper.Map<GetByIdColorDTO>(value);
            return Ok(result);
        }
    }
}