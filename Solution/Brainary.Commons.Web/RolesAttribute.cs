using Microsoft.AspNetCore.Authorization;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// <see cref="AuthorizeAttribute"/> implementation for roles by default.
    /// </summary>
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute() { }

        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
