using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BrandDTO.Messages
{
    public class BrandUpdatedMessage
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }
}
