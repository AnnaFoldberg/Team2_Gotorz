using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    public class Navigation
    {
        [JsonPropertyName("entityId")]
        public string EntityId { get; set; }

        [JsonPropertyName("entityType")]
        public string EntityType { get; set; }

        [JsonPropertyName("localizedName")]
        public string LocalizedName { get; set; }

        [JsonPropertyName("relevantFlightParams")]
        public Airport RelevantFlightParams { get; set; }
    }
}
