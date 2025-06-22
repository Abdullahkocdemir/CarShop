using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.ColorDTO
{
    public class UpdateColorDTO
    {
        public int ColorId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
