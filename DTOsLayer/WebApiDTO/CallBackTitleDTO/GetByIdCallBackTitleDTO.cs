using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.CallBackTitleDTO
{
    public class GetByIdCallBackTitleDTO
    {
        public int CallBackTitleId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
