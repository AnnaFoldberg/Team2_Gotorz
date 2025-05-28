using System.ComponentModel.DataAnnotations;

public class HotelSearchForm
    {
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;

        [Required]
        public DateTime ArrivalDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(1);
    }
