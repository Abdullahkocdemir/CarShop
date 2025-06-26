using AutoMapper;
using DTOsLayer.WebApiDTO.Account;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
// highlights-start
using Microsoft.EntityFrameworkCore; // <--- EKLENMESİ GEREKEN SATIR
// highlights-end

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new AppRole
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Rol başarıyla oluşturuldu." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(assignRoleDto.RoleName);
            if (!roleExists)
            {
                return NotFound("Rol bulunamadı.");
            }

            var result = await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Kullanıcıya rol başarıyla atandı." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole(AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var result = await _userManager.RemoveFromRoleAsync(user, assignRoleDto.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Kullanıcıdan rol başarıyla kaldırıldı." });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            // highlights-start
            var roles = await _roleManager.Roles.ToListAsync(); // <--- HATA VEREN SATIRDI
            var roleDtos = _mapper.Map<List<RoleListDto>>(roles); // <--- DÜZELTİLMESİ GEREKEN İKİNCİ SATIR
                                                                  // highlights-end
            return Ok(roleDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound("Rol bulunamadı.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Rol başarıyla silindi." });
            }

            return BadRequest(result.Errors);
        }
    }
}