using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.StaffDTO
{
    public class UpdateStaffDTO
    {
        public int StaffId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Duty { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } 
        public string? ExistingImageUrl { get; set; } 
    }
}
