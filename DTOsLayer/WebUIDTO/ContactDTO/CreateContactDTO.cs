using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.ContactDTO
{
    public class CreateContactDTO
    {
        public string NameSurName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
