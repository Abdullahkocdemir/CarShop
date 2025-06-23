using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BlogDTO
{
    public class GetByIdBlogDTO
    {
        public int BlogId { get; set; }
        public string BannerImageUrl { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string SmallDescription { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CommentCount { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool PopulerBlog { get; set; }
    }
}
