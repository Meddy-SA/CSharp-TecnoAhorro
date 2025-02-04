using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Models.Authentication;

public class AppRole : IdentityRole
{
    public virtual ICollection<AppUserRole> UserRoles { get; set; }

    public AppRole()
        : base()
    {
        UserRoles = new HashSet<AppUserRole>();
    }

    public AppRole(string roleName)
        : base(roleName)
    {
        UserRoles = new HashSet<AppUserRole>();
    }
}
