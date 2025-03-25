using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    public class AutoCompleteResponse
    {
        [JsonPropertyName("inputSuggest")]
        public List<InputSuggest> InputSuggest { get; set; }
    }
}
