using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BlogDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : BaseEntityController
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "Blog";

        public BlogsController(IBlogService blogService, EnhancedRabbitMQService rabbitMqService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _blogService = blogService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllBlogs()
        {
            var values = _blogService.BGetListAll();
            var result = values.Select(blog => new ResultBlogDTO
            {
                BlogId = blog.BlogId,
                BannerImageUrl = !string.IsNullOrEmpty(blog.BannerImageUrl) ? $"{Request.Scheme}://{Request.Host}/blogs/{blog.BannerImageUrl}" : string.Empty,
                SmallTitle = blog.SmallTitle,
                Author = blog.Author,
                SmallDescription = blog.SmallDescription,
                Date = blog.Date,
                CommentCount = blog.CommentCount,
                Title = blog.Title,
                ImageUrl = !string.IsNullOrEmpty(blog.ImageUrl) ? $"{Request.Scheme}://{Request.Host}/blogs/{blog.ImageUrl}" : string.Empty,
                Description = blog.Description
            }).ToList();

            return Ok(result);
        }


        [HttpPost]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDTO dto)
        {
            var blog = _mapper.Map<Blog>(dto);

            if (dto.BannerImage != null)
            {
                blog.BannerImageUrl = await SaveImage(dto.BannerImage);
            }

            if (dto.MainImage != null)
            {
                blog.ImageUrl = await SaveImage(dto.MainImage);
            }

            _blogService.BAdd(blog);
            PublishEntityCreated(blog); 
            return Ok(new { Message = "Blog başarıyla eklendi ve mesaj yayınlandı.", BlogId = blog.BlogId });
        }
        [HttpPut]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UpdateBlog([FromForm] UpdateBlogDTO dto)
        {
            var existingBlog = _blogService.BGetById(dto.BlogId);

            if (existingBlog == null)
            {
                return NotFound($"Blog with ID {dto.BlogId} bulunamadı.");
            }

            _mapper.Map(dto, existingBlog); 

            if (dto.BannerImage != null)
            {
                if (!string.IsNullOrEmpty(existingBlog.BannerImageUrl))
                {
                    DeleteImage(existingBlog.BannerImageUrl); 
                }
                existingBlog.BannerImageUrl = await SaveImage(dto.BannerImage); 
            }

            if (dto.MainImage != null)
            {
                if (!string.IsNullOrEmpty(existingBlog.ImageUrl))
                {
                    DeleteImage(existingBlog.ImageUrl); 
                }
                existingBlog.ImageUrl = await SaveImage(dto.MainImage); 
            }

            _blogService.BUpdate(existingBlog);
            PublishEntityUpdated(existingBlog);

            return Ok(new { Message = "Blog güncellendi ve mesaj yayınlandı.", BlogId = existingBlog.BlogId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBlog(int id)
        {
            var blog = _blogService.BGetById(id);
            if (blog == null)
            {
                return NotFound($"Blog with ID {id} bulunamadı.");
            }

            if (!string.IsNullOrEmpty(blog.BannerImageUrl))
            {
                DeleteImage(blog.BannerImageUrl);
            }
            if (!string.IsNullOrEmpty(blog.ImageUrl))
            {
                DeleteImage(blog.ImageUrl);
            }

            _blogService.BDelete(blog);
            PublishEntityDeleted(blog); 
            return Ok(new { Message = "Blog silindi ve mesaj yayınlandı.", BlogId = id });
        }
        [HttpGet("{id}")]
        public IActionResult GetByIdBlog(int id)
        {
            var value = _blogService.BGetById(id);
            if (value == null)
            {
                return NotFound($"Blog with ID {id} bulunamadı.");
            }

            var result = new GetByIdBlogDTO
            {
                BlogId = value.BlogId,
                BannerImageUrl = !string.IsNullOrEmpty(value.BannerImageUrl) ? $"{Request.Scheme}://{Request.Host}/blogs/{value.BannerImageUrl}" : string.Empty,
                SmallTitle = value.SmallTitle,
                Author = value.Author,
                SmallDescription = value.SmallDescription,
                Date = value.Date,
                CommentCount = value.CommentCount,
                Title = value.Title,
                ImageUrl = !string.IsNullOrEmpty(value.ImageUrl) ? $"{Request.Scheme}://{Request.Host}/blogs/{value.ImageUrl}" : string.Empty,
                Description = value.Description
            };
            return Ok(result);
        }


        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty;
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "blogs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private void DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "blogs", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}