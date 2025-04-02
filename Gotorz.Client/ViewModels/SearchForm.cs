using System.ComponentModel.DataAnnotations;

namespace Gotorz.Client.ViewModels
{
    /// <summary>
    /// Represents the search form used in the flight search UI.
    /// </summary>
    public class SearchForm
    {
        [DateFormat("dd-MM-yyyy")]
        public string? date { get; set; }

        [Required(ErrorMessage = "Departure airport is required.")]
        public string departureAirport { get; set; }= string.Empty;

        [Required(ErrorMessage = "Arrival airport is required.")]
        public string arrivalAirport { get; set; }= string.Empty;
    }
}