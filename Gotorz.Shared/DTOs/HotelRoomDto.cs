using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a room used for data transfer between the client and server.
    /// </summary>
    public class HotelRoomDto
    {
        [JsonIgnore]
        public int HotelRoomId { get; set; }
        public string ExternalRoomId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string? MealPlan { get; set; }
        public bool Refundable { get; set; }
        public HotelDto Hotel { get; set; }
    }
}