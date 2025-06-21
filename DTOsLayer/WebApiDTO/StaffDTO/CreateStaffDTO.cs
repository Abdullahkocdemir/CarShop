using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.StaffDTO
{
    public class CreateStaffDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Duty { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } 
    }
}
