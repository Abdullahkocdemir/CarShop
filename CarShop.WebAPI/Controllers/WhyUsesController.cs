using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ; // Bu servisin projenizde tanımlı olduğunu varsaydım
using DTOsLayer.WebApiDTO.WhyUseDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq; 

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhyUsesController : BaseEntityController 
    {
        private readonly IWhyUseService _whyUseService;
        private readonly IMapper _mapper;

        protected override string EntityTypeName => "WhyUse"; 

        public WhyUsesController(IWhyUseService whyUseService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService) 
        {
            _whyUseService = whyUseService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListWhyUse()
        {
            var whyUses = _whyUseService.BGetWhyUseWithItem();
            var whyUseDtos = _mapper.Map<List<ResultWhyUseDTO>>(whyUses);
            return Ok(whyUseDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdWhyUse(int id)
        {
            var whyUse = _whyUseService.BGetById(id);
            if (whyUse == null)
            {
                return NotFound($"WhyUse with ID {id} not found.");
            }
            var whyUseDto = _mapper.Map<GetByIdWhyUseDTO>(whyUse);
            return Ok(whyUseDto);
        }

        [HttpPost]
        public IActionResult CreateWhyUse(CreateWhyUseDTO dto)
        {
            var whyUse = _mapper.Map<WhyUse>(dto);
            whyUse.Items = new List<WhyUseItem>();
            if (dto.Items != null)
            {
                foreach (var itemDto in dto.Items)
                {
                    whyUse.Items.Add(new WhyUseItem { Content = itemDto.Content });
                }
            }

            _whyUseService.BAdd(whyUse);
            PublishEntityCreated(whyUse); 

            return Ok(new { Message = "WhyUse başarıyla eklendi ve mesaj yayınlandı.", WhyUseId = whyUse.WhyUseId });
        }

        [HttpPut]
        public IActionResult UpdateWhyUse(UpdateWhyUseDTO dto)
        {
            var existingWhyUse = _whyUseService.BGetById(dto.WhyUseId);
            if (existingWhyUse == null)
            {
                return NotFound($"WhyUse with ID {dto.WhyUseId} not found.");
            }

            _mapper.Map(dto, existingWhyUse);
            if (dto.Items != null)
            {
                var existingItemIds = existingWhyUse.Items.Select(i => i.Id).ToList();
                var incomingItemIds = dto.Items.Select(i => i.Id).ToList();

                foreach (var existingItem in existingWhyUse.Items.ToList()) 
                {
                    if (!incomingItemIds.Contains(existingItem.Id))
                    {
                        existingWhyUse.Items.Remove(existingItem);
                    }
                }

                foreach (var incomingItemDto in dto.Items)
                {
                    if (incomingItemDto.Id > 0) 
                    {
                        var existingItem = existingWhyUse.Items.FirstOrDefault(i => i.Id == incomingItemDto.Id);
                        if (existingItem != null)
                        {
                            _mapper.Map(incomingItemDto, existingItem); 
                        }
                    }
                    else 
                    {
                        existingWhyUse.Items.Add(_mapper.Map<WhyUseItem>(incomingItemDto));
                    }
                }
            }
            else
            {
                existingWhyUse.Items.Clear();
            }

            _whyUseService.BUpdate(existingWhyUse); 
            PublishEntityUpdated(existingWhyUse); 

            return Ok(new { Message = "WhyUse başarıyla güncellendi ve mesaj yayınlandı.", WhyUseId = existingWhyUse.WhyUseId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWhyUse(int id)
        {
            var whyUse = _whyUseService.BGetById(id);
            if (whyUse == null)
            {
                return NotFound($"WhyUse with ID {id} not found.");
            }
            _whyUseService.BDelete(whyUse); 
            PublishEntityDeleted(whyUse); 

            return Ok(new { Message = "WhyUse başarıyla silindi ve mesaj yayınlandı.", WhyUseId = id });
        }
    }
}
