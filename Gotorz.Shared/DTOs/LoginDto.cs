using System.ComponentModel.DataAnnotations;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a login request used for data transfer between the client and server.
    /// </summary>
    /// <author>Eske</author>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
