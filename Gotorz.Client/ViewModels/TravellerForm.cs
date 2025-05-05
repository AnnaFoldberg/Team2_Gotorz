using System.ComponentModel.DataAnnotations;

namespace Gotorz.Client.ViewModels
{
    /// <summary>
    /// Represents the traveller form used in the travellers UI.
    /// </summary>
    /// <author>Anna</author>
    public class TravellerForm
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Passport number is required")]
        public string PassportNumber { get; set; } = string.Empty;
    }
}