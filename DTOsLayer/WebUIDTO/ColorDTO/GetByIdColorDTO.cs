using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.ColorDTO
{
    public class GetByIdColorDTO
    {
        public int ColorId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
