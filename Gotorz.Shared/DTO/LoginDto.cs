using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTO
{
    /// <summary>
    /// Represents a login request used for data transfer between the client and server.
    /// </summary>
    /// <author>Eske</author>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Ugyldig email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        public string Password { get; set; } = string.Empty;
    }
}
