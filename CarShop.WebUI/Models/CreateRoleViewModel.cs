using System.ComponentModel.DataAnnotations;

namespace CarShop.WebUI.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }
    }
}
