using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class BlogDetails
    {
        public int BlogDetailsId { get; set; }
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }
        public DateTime Date { get; set; } 
        public int CountCommnet { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
