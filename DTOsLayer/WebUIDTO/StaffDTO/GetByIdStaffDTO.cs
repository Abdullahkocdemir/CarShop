using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.StaffDTO
{
    public class GetByIdStaffDTO
    {
        public int StaffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Duty { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
