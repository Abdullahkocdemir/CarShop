using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTOsLayer.WebApiDTO.TestimonialDTO;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialsController : BaseEntityController
    {
        private readonly ITestimonialService _testimonialService; 
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        protected override string EntityTypeName => "Testimonial";

        public TestimonialsController(
            ITestimonialService testimonialService, 
            EnhancedRabbitMQService rabbitMqService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _testimonialService = testimonialService; 
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetListTestimonial()
        {
            var values = _testimonialService.BGetListAll();
            var resultDto = _mapper.Map<List<ResultTestimonialDTO>>(values);
            return Ok(resultDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdTestimonial(int id)
        {
            var value = _testimonialService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan Testimonial bulunamadı.");
            }
            var resultDto = _mapper.Map<GetByIdTestimonialDTO>(value);
            return Ok(resultDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestimonial([FromForm] CreateTestimonialDTO createTestimonialDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var testimonial = _mapper.Map<Testimonial>(createTestimonialDto);

            if (createTestimonialDto.ImageFile != null && createTestimonialDto.ImageFile.Length > 0)
            {
                testimonial.ImageUrl = await SaveImage(createTestimonialDto.ImageFile, "Testimonial");
            }
            else
            {
                return BadRequest("Resim dosyası yüklenmelidir.");
            }

            _testimonialService.BAdd(testimonial);
            PublishEntityCreated(testimonial);

            return StatusCode(201, new { Message = "Testimonial başarıyla eklendi ve mesaj gönderildi.", TestimonialId = testimonial.TestimonialId, ImageUrl = testimonial.ImageUrl });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTestimonial([FromForm] UpdateTestimonialDTO updateTestimonialDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTestimonial = _testimonialService.BGetById(updateTestimonialDto.TestimonialId);
            if (existingTestimonial == null)
            {
                return NotFound($"ID'si {updateTestimonialDto.TestimonialId} olan Testimonial bulunamadı.");
            }

            _mapper.Map(updateTestimonialDto, existingTestimonial);

            if (updateTestimonialDto.ImageFile != null && updateTestimonialDto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingTestimonial.ImageUrl))
                {
                    DeleteImage(existingTestimonial.ImageUrl, "Testimonial");
                }
                existingTestimonial.ImageUrl = await SaveImage(updateTestimonialDto.ImageFile, "Testimonial");
            }
            else if (!string.IsNullOrEmpty(updateTestimonialDto.ExistingImageUrl))
            {
                existingTestimonial.ImageUrl = updateTestimonialDto.ExistingImageUrl;
            }


            _testimonialService.BUpdate(existingTestimonial);
            PublishEntityUpdated(existingTestimonial);

            return Ok(new { Message = "Testimonial başarıyla güncellendi ve mesaj yayınlandı.", TestimonialId = existingTestimonial.TestimonialId, ImageUrl = existingTestimonial.ImageUrl });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTestimonial(int id)
        {
            var value = _testimonialService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan Testimonial bulunamadı.");
            }

            if (!string.IsNullOrEmpty(value.ImageUrl))
            {
                DeleteImage(value.ImageUrl, "Testimonial");
            }

            _testimonialService.BDelete(value);
            PublishEntityDeleted(value);

            return Ok(new { Message = "Testimonial başarıyla silindi ve mesaj yayınlandı.", TestimonialId = id });
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