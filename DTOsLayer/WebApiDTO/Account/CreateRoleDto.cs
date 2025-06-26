using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.Account
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
