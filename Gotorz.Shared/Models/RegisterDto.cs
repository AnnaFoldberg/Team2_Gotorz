using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Ugyldig email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fornavn er påkrævet")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Efternavn er påkrævet")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer er påkrævet")]
        [Phone(ErrorMessage = "Ugyldigt telefonnummer")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Adgangskoden skal være mindst 6 tegn.")]
        [RegularExpression(@"^(?=.*[a-zæøå])(?=.*[A-ZÆØÅ]).+$", ErrorMessage = "Adgangskoden skal indeholde både store og små bogstaver.")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
