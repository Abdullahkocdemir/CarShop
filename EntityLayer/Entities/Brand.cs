using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{   
    public class Brand
    {
        public int BrandId { get; set; }
        [Required]
        [Display(Name = "Marka")]
        [StringLength(15)]
        public string BrandName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
