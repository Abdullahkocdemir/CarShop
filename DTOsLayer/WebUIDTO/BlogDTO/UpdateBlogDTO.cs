using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BlogDTO
{
    public class UpdateBlogDTO
    {
        public int BlogId { get; set; }
        public IFormFile? BannerImage { get; set; }
        public string? ExistingBannerImageUrl { get; set; } 
        public string SmallTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string SmallDescription { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CommentCount { get; set; }
        public string Title { get; set; } = string.Empty;
        public IFormFile? MainImage { get; set; }
        public string? ExistingMainImageUrl { get; set; } // Mevcut ana resim URL'si
        public string Description { get; set; } = string.Empty;
        public bool PopulerBlog { get; set; }
    }
}
