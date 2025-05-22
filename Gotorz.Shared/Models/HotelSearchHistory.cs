using System;
using System.ComponentModel.DataAnnotations;

namespace Gotorz.Shared.Models
{
    public class HotelSearchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public DateTime DepartureDate { get; set; }

        public DateTime SearchTimestamp { get; set; } = DateTime.UtcNow;
    }
}
