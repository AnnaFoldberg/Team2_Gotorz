using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gotorz.Shared.Models
{
    public class HotelRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelRoomId { get; set; }
        public ICollection<HotelBooking> HotelBookings { get; set; }


        // Ekstern hotel-ID fra API'et, bruges til at knytte værelser til hoteller
        public string ExternalHotelId { get; set; } = string.Empty;

        // Værelsets unikke ID fra API'et
        public string RoomId { get; set; } = string.Empty;

        // Navn på værelset
        public string Name { get; set; } = string.Empty;

        // Beskrivelse af værelset
        public string? Description { get; set; }

        // Maksimal kapacitet (antal personer)
        public int Capacity { get; set; }

        // Størrelse på værelset i m2
        public int Surface { get; set; }

        // Pris for hele opholdet i valgt periode
        public decimal Price { get; set; }

        // Information om måltider (morgenmad, halvpension osv.)
        public string? MealPlan { get; set; }

        // Om reservationen kan refunderes
        public bool Refundable { get; set; }

        // Beskrivelse af afbestillingspolitikken
        public string? CancellationPolicy { get; set; }

        // Ankomstdato
        public DateTime ArrivalDate { get; set; }

        // Afrejsedato
        public DateTime DepartureDate { get; set; }
    }
}