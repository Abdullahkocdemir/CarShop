using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BrandDTO
{
    public class ResultBrandDTO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
