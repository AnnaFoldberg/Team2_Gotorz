using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents a holiday package used for data transfer between the client and server.
    /// </summary>
    public class HolidayPackageDto
    {
        // HolidayPackageId needed internally for mapping holiday bookings.
        // [JsonIgnore] ensures it won't be included in JSON responses to the client.
        // It also hides it from API docs generated in Swagger.
        [JsonIgnore]
        public int HolidayPackageId { get; set; }
        public string Destination { get; set; }
        public int MaxCapacity { get; set; }
    }
}