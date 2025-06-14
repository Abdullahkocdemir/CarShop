﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.ProductDTOs.DTO
{
    public class UpdateProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
