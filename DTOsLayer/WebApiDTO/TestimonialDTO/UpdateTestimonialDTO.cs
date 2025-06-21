using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.TestimonialDTO
{
    public class UpdateTestimonialDTO
    {
        public int TestimonialId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Duty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
