using AutoMapper;
using BusinessLayer.Abstract; 
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.AboutDTO; 
using EntityLayer.Entities; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial;
using SharpCompress.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutsController : BaseEntityController
    {
        private readonly IAboutService _aboutService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "About"; 

        public AboutsController(IAboutService aboutService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _aboutService = aboutService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAbout()
        {
            var abouts = _aboutService.BGetListAll();
            var aboutDtos = _mapper.Map<List<ResultAboutDTO>>(abouts);
            return Ok(aboutDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdAbout(int id)
        {
            var about = _aboutService.BGetById(id);
            if (about == null)
            {
                return NotFound($"About entry with ID {id} not found.");
            }
            var aboutDto = _mapper.Map<GetByIdAboutDTO>(about);
            return Ok(aboutDto);
        }

        [HttpPost]
        public IActionResult CreateAbout(CreateAboutDTO dto)
        {
            var about = _mapper.Map<About>(dto);
            _aboutService.BAdd(about);
            PublishEntityCreated(about); 

            return Ok(new { Message = "About girişi başarıyla eklendi ve mesaj yayınlandı.", AboutId = about.AboutId });
        }
        [HttpPut]
        public IActionResult UpdateAbout(UpdateAboutDTO dto)
        {
            var existingAbout = _aboutService.BGetById(dto.AboutId);
            if (existingAbout == null)
            {
                return NotFound($"About entry with ID {dto.AboutId} not found for update.");
            }
            _mapper.Map(dto, existingAbout); 
            _aboutService.BUpdate(existingAbout);
            PublishEntityUpdated(existingAbout);

            return Ok(new { Message = "About girişi başarıyla güncellendi ve mesaj yayınlandı.", AboutId = existingAbout.AboutId });
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteAbout(int id)
        {
            var about = _aboutService.BGetById(id);
            if (about == null)
            {
                return NotFound($"About entry with ID {id} not found for deletion.");
            }
            _aboutService.BDelete(about);
            PublishEntityDeleted(about); 

            return Ok(new { Message = "About girişi başarıyla silindi ve mesaj yayınlandı.", AboutId = id });
        }
        
    }
}