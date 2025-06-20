using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.CalltoActionDTO
{
    public class ResultCalltoActionDTO
    {
        public int CalltoActionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; 
    }
}
