using System.Text.Json.Serialization;

namespace Gotorz.Shared.DTOs
{
    /// <summary>
    /// Represents an airport used for data transfer between the client and server.
    /// </summary>
    /// <author>Anna</author>
    public class AirportDto
    {
        // AirportId needed internally for mapping flights.
        // [JsonIgnore] ensures it won't be included in JSON responses to the client.
        // It also hides it from API docs generated in Swagger.
        [JsonIgnore]
        public int AirportId { get; set; }
        public string EntityId { get; set; }
        public string LocalizedName { get; set; }
        public string SkyId { get; set; }
    }
}