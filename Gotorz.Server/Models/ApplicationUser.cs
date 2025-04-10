using Microsoft.AspNetCore.Identity;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents the application user.
    /// </summary>
    /// <author>Eske</author>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Additional properties for the application user, other than the default IdentityUser properties.
        /// </summary>
        /// <author>Eske</author>
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
