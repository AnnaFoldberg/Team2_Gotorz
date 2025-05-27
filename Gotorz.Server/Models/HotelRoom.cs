using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gotorz.Server.Models
{
    public class HotelRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelRoomId { get; set; }

        public Hotel? Hotel { get; set; } = null;

        public int HotelId { get; set; } 

        // Værelsets unikke ID fra API'et
        public string ExternalRoomId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Capacity { get; set; }

        public decimal Price { get; set; }

        public string? MealPlan { get; set; }

        public bool Refundable { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }
    }
}