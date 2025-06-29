﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.FeatureDTO
{
    public class GetByIdFeatureDTO
    {
        public int FeatureId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
