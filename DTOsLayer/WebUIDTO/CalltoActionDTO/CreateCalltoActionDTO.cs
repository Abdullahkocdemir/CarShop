using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.CalltoActionDTO
{
    public class CreateCalltoActionDTO
    {
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } 
    }
}
