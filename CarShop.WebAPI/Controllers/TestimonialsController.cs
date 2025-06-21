using AutoMapper;
using BusinessLayer.Abstract; // Assuming ITestimonialService is here
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.TestimonialDTO; // Your Testimonial DTOs
using EntityLayer.Entities; // Your Testimonial entity
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
    public class TestimonialsController : BaseEntityController
    {
        private readonly ITestimonialService _testimonialService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Testimonial";

        public TestimonialsController(ITestimonialService testimonialService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _testimonialService = testimonialService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListTestimonial()
        {
            var testimonials = _testimonialService.BGetListAll();
            var testimonialDtos = _mapper.Map<List<ResultTestimonialDTO>>(testimonials);
            return Ok(testimonialDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdTestimonial(int id)
        {
            var testimonial = _testimonialService.BGetById(id);
            if (testimonial == null)
            {
                return NotFound($"ID'si {id} olan referans bulunamadı.");
            }
            var testimonialDto = _mapper.Map<GetByIdTestimonialDTO>(testimonial);
            return Ok(testimonialDto);
        }

        [HttpPost]
        public IActionResult CreateTestimonial(CreateTestimonialDTO dto)
        {
            var testimonial = _mapper.Map<Testimonial>(dto);
            _testimonialService.BAdd(testimonial);
            PublishEntityCreated(testimonial);

            return Ok(new { Message = "Referans başarıyla eklendi ve mesaj yayınlandı.", TestimonialId = testimonial.TestimonialId });
        }

        [HttpPut]
        public IActionResult UpdateTestimonial(UpdateTestimonialDTO dto)
        {
            var existingTestimonial = _testimonialService.BGetById(dto.TestimonialId);
            if (existingTestimonial == null)
            {
                return NotFound($"ID'si {dto.TestimonialId} olan referans bulunamadı.");
            }
            _mapper.Map(dto, existingTestimonial);
            _testimonialService.BUpdate(existingTestimonial);
            PublishEntityUpdated(existingTestimonial);

            return Ok(new { Message = "Referans başarıyla güncellendi ve mesaj yayınlandı.", TestimonialId = existingTestimonial.TestimonialId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTestimonial(int id)
        {
            var testimonial = _testimonialService.BGetById(id);
            if (testimonial == null)
            {
                return NotFound($"ID'si {id} olan referans bulunamadı.");
            }
            _testimonialService.BDelete(testimonial);
            PublishEntityDeleted(testimonial);

            return Ok(new { Message = "Referans başarıyla silindi ve mesaj yayınlandı.", TestimonialId = id });
        }

    }
}