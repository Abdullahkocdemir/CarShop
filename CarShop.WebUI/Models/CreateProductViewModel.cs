using DTOsLayer.WebUIDTO.ProductDTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarShop.WebUI.Models
{
    public class CreateProductViewModel
    {
        public CreateProductDTO Product { get; set; } = new CreateProductDTO();
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Models { get; set; } = new List<SelectListItem>();
    }
}
