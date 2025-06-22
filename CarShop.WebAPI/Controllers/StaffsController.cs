using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.StaffDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : BaseEntityController
    {
        private readonly IStaffService _staffService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "Staff";

        public StaffsController(IStaffService staffService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _staffService = staffService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllStaffs()
        {
            var staffs = _staffService.BGetListAll();
            var staffDtos = _mapper.Map<List<ResultStaffDTO>>(staffs);
            return Ok(staffDtos);
        }
        [HttpGet("{id}")]
        public IActionResult GetStaffById(int id)
        {
            var staff = _staffService.BGetById(id);
            if (staff == null)
            {
                return NotFound($"ID'si {id} olan personel bulunamadı.");
            }
            var staffDto = _mapper.Map<GetByIdStaffDTO>(staff);
            return Ok(staffDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromForm] CreateStaffDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staff = _mapper.Map<Staff>(dto);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                staff.ImageUrl = await SaveImage(dto.ImageFile, "staffs"); 
            }
            else
            {
                return BadRequest("Personel oluşturmak için bir resim dosyası gereklidir.");
            }

            _staffService.BAdd(staff);
            PublishEntityCreated(staff);

            return StatusCode(201, new { Message = "Personel başarıyla eklendi ve mesaj gönderildi.", StaffId = staff.StaffId, ImageUrl = staff.ImageUrl });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStaff([FromForm] UpdateStaffDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingStaff = _staffService.BGetById(dto.StaffId);
            if (existingStaff == null)
            {
                return NotFound($"ID'si {dto.StaffId} olan personel bulunamadı.");
            }

            _mapper.Map(dto, existingStaff);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl, "staffs");
                }
                existingStaff.ImageUrl = await SaveImage(dto.ImageFile, "staffs");
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingStaff.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl, "staffs");
                }
                existingStaff.ImageUrl = string.Empty;
            }

            _staffService.BUpdate(existingStaff);
            PublishEntityUpdated(existingStaff);

            return Ok(new { Message = "Personel başarıyla güncellendi ve mesaj yayınlandı.", StaffId = existingStaff.StaffId, ImageUrl = existingStaff.ImageUrl });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStaff(int id)
        {
            var staffToDelete = _staffService.BGetById(id);
            if (staffToDelete == null)
            {
                return NotFound($"ID'si {id} olan personel bulunamadı.");
            }

            if (!string.IsNullOrEmpty(staffToDelete.ImageUrl))
            {
                DeleteImage(staffToDelete.ImageUrl, "staffs");
            }

            _staffService.BDelete(staffToDelete);
            PublishEntityDeleted(staffToDelete);

            return Ok(new { Message = "Personel başarıyla silindi ve mesaj yayınlandı.", StaffId = id });
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
