using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a registration request used for data transfer between the client and server.
    /// </summary>
    /// <author>Eske</author>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phonenumber is required")]
        [Phone(ErrorMessage = "Invalid phonenumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-zæøå])(?=.*[A-ZÆØÅ]).+$", ErrorMessage = "Password must include both uppercase and lowercase letters")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
