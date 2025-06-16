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

        [Required]
        [Display(Name = "Ülke")]
        [StringLength(15)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Yıl")]
        [StringLength(4)]
        public string EstablishmentYear { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        // Marka ürünleri
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
