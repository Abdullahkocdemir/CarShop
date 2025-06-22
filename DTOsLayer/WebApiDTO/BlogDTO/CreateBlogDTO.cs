using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BlogDTO
{
    public class CreateBlogDTO
    {
        public IFormFile? BannerImage { get; set; } 
        public string SmallTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string SmallDescription { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now; 
        public int CommentCount { get; set; } = 0; 
        public string Title { get; set; } = string.Empty;
        public IFormFile? MainImage { get; set; } 
        public string Description { get; set; } = string.Empty;
    }
}
