﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BrandDTO
{
    public class GetByIdBrandDTO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
