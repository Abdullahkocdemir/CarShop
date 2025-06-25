using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.TestimonialDTO
{
    public class GetByIdTestimonialDTO
    {
        public int TestimonialId { get; set; }
        public string NameSurname { get; set; } = string.Empty;
        public string Duty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
