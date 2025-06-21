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
            var staff = _mapper.Map<Staff>(dto);

            if (dto.ImageFile != null)
            {
                staff.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else
            {
                staff.ImageUrl = string.Empty;
            }

            _staffService.BAdd(staff);
            PublishEntityCreated(staff);

            return Ok(new { Message = "Personel başarıyla eklendi ve mesaj gönderildi.", StaffId = staff.StaffId, ImageUrl = staff.ImageUrl });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStaff([FromForm] UpdateStaffDTO dto)
        {
            var existingStaff = _staffService.BGetById(dto.StaffId);
            if (existingStaff == null)
            {
                return NotFound($"ID'si {dto.StaffId} olan personel bulunamadı.");
            }

            _mapper.Map(dto, existingStaff);

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl);
                }
                existingStaff.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingStaff.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                if (!string.IsNullOrEmpty(existingStaff.ImageUrl))
                {
                    DeleteImage(existingStaff.ImageUrl);
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
                DeleteImage(staffToDelete.ImageUrl);
            }

            _staffService.BDelete(staffToDelete);
            PublishEntityDeleted(staffToDelete);

            return Ok(new { Message = "Personel başarıyla silindi ve mesaj yayınlandı.", StaffId = id });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "staffs");
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

            return $"/staffs/{uniqueFileName}"; ;
        }

        private void DeleteImage(string imageUrl)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "staffs", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}