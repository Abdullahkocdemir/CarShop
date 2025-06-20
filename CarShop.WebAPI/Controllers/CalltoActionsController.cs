using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.CalltoActionDTO; 
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
        public IActionResult GetListAllCalltoActions()
        {
            var calltoActions = _calltoActionService.BGetListAll();
            var calltoActionDtos = _mapper.Map<List<ResultCalltoActionDTO>>(calltoActions);
            return Ok(calltoActionDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetCalltoActionById(int id)
        {
            var calltoAction = _calltoActionService.BGetById(id);
            if (calltoAction == null)
            {
                return NotFound($"ID'si {id} olan Call to Action bulunamadı.");
            }
            var calltoActionDto = _mapper.Map<GetByIdCalltoActionDTO>(calltoAction);
            return Ok(calltoActionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCalltoAction([FromForm] CreateCalltoActionDTO dto) 
        {
            var calltoAction = _mapper.Map<CalltoAction>(dto);

            if (dto.ImageFile != null)
            {
                calltoAction.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else
            {
                calltoAction.ImageUrl = string.Empty;
            }

            _calltoActionService.BAdd(calltoAction);
            PublishEntityCreated(calltoAction);

            return Ok(new { Message = "Call to Action başarıyla eklendi ve mesaj gönderildi.", CalltoActionId = calltoAction.CalltoActionId, ImageUrl = calltoAction.ImageUrl });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCalltoAction([FromForm] UpdateCalltoActionDTO dto) 
        {
            var existingCalltoAction = _calltoActionService.BGetById(dto.CalltoActionId);
            if (existingCalltoAction == null)
            {
                return NotFound($"ID'si {dto.CalltoActionId} olan Call to Action bulunamadı.");
            }
            _mapper.Map(dto, existingCalltoAction);

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingCalltoAction.ImageUrl))
                {
                    DeleteImage(existingCalltoAction.ImageUrl);
                }
                existingCalltoAction.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingCalltoAction.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                existingCalltoAction.ImageUrl = string.Empty;
            }

            _calltoActionService.BUpdate(existingCalltoAction);
            PublishEntityUpdated(existingCalltoAction);

            return Ok(new { Message = "Call to Action başarıyla güncellendi ve mesaj yayınlandı.", CalltoActionId = existingCalltoAction.CalltoActionId, ImageUrl = existingCalltoAction.ImageUrl });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCalltoAction(int id)
        {
            var calltoActionToDelete = _calltoActionService.BGetById(id);
            if (calltoActionToDelete == null)
            {
                return NotFound($"ID'si {id} olan Call to Action bulunamadı.");
            }

            // Delete the associated image file
            if (!string.IsNullOrEmpty(calltoActionToDelete.ImageUrl))
            {
                DeleteImage(calltoActionToDelete.ImageUrl);
            }

            _calltoActionService.BDelete(calltoActionToDelete);
            PublishEntityDeleted(calltoActionToDelete);

            return Ok(new { Message = "Call to Action başarıyla silindi ve mesaj yayınlandı.", CalltoActionId = id });
        }
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "calltoaction");
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
            return $"/calltoaction/{uniqueFileName}";

        }

        private void DeleteImage(string imageUrl)
        {
            var fileName = Path.GetFileName(imageUrl.Replace("/calltoaction/", ""));
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "calltoaction", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

    }
}