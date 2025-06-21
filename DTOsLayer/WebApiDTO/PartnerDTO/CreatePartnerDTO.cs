using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.PartnerDTO
{
    public class CreatePartnerDTO
    {
        public IFormFile? ImageFile { get; set; } 
    }
}
