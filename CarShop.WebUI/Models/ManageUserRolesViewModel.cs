namespace CarShop.WebUI.Models
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<RoleCheckBox> Roles { get; set; } = new List<RoleCheckBox>();
    }

    public class RoleCheckBox
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}

public class UserDetailDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public IList<string>? Roles { get; set; }
}
public class RoleListDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

