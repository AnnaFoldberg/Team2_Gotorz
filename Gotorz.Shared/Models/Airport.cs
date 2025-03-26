using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    public class Airport
    {
        public int AirportId { get; set; }
        [JsonPropertyName("id")]
        public string EntityId { get; set; }
        [JsonPropertyName("name")]
        public string LocalizedName { get; set; }
        [JsonPropertyName("skyCode")]
        public string SkyId { get; set; }
    }
}