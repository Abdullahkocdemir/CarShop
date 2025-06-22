using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DTOsLayer.WebApiDTO.CalltoActionDTO;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalltoActionsController : BaseEntityController 
    {
        private readonly ICalltoActionService _calltoActionService; 
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "CalltoAction"; 

        public CalltoActionsController(ICalltoActionService calltoActionService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _calltoActionService = calltoActionService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllGotus()
        {
            var gotus = _calltoActionService.BGetListAll();
            var gotoDtos = _mapper.Map<List<ResultCalltoActionDTO>>(gotus);
            return Ok(gotoDtos);
        }
        [HttpGet("{id}")]
        public IActionResult GetGotoById(int id)
        {
            var @goto = _calltoActionService.BGetById(id); 
            if (@goto == null)
            {
                return NotFound($"ID'si {id} olan Goto bulunamadı.");
            }
            var gotoDto = _mapper.Map<GetByIdCalltoActionDTO>(@goto);
            return Ok(gotoDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGoto([FromForm] CreateCalltoActionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @goto = _mapper.Map<CalltoAction>(dto);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                @goto.ImageUrl = await SaveImage(dto.ImageFile, "goto"); 
            }
            else
            {
                return BadRequest("Resim dosyası yüklenmelidir.");
            }

            _calltoActionService.BAdd(@goto);
            PublishEntityCreated(@goto); 

            return StatusCode(201, new { Message = "Goto başarıyla eklendi ve mesaj gönderildi.", GotoId = @goto.CalltoActionId, ImageUrl = @goto.ImageUrl });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateGoto([FromForm] UpdateCalltoActionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGoto = _calltoActionService.BGetById(dto.CalltoActionId);
            if (existingGoto == null)
            {
                return NotFound($"ID'si {dto.CalltoActionId} olan Goto bulunamadı.");
            }

            _mapper.Map(dto, existingGoto); 

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingGoto.ImageUrl))
                {
                    DeleteImage(existingGoto.ImageUrl, "goto");
                }
                existingGoto.ImageUrl = await SaveImage(dto.ImageFile, "goto"); 
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingGoto.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                existingGoto.ImageUrl = string.Empty;
            }

            _calltoActionService.BUpdate(existingGoto);
            PublishEntityUpdated(existingGoto); 

            return Ok(new { Message = "Goto başarıyla güncellendi ve mesaj yayınlandı.", GotoId = existingGoto.CalltoActionId, ImageUrl = existingGoto.ImageUrl });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteGoto(int id)
        {
            var gotoToDelete = _calltoActionService.BGetById(id);
            if (gotoToDelete == null)
            {
                return NotFound($"ID'si {id} olan Goto bulunamadı.");
            }

            if (!string.IsNullOrEmpty(gotoToDelete.ImageUrl))
            {
                DeleteImage(gotoToDelete.ImageUrl, "goto");
            }

            _calltoActionService.BDelete(gotoToDelete);
            PublishEntityDeleted(gotoToDelete); 

            return Ok(new { Message = "Goto başarıyla silindi ve mesaj yayınlandı.", GotoId = id });
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