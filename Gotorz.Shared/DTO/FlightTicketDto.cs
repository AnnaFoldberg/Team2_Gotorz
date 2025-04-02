using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTO
{
    /// <summary>
    /// Represents the data structure for a <see cref="FlightTicketDto"/> entity.
    /// </summary>
    public class FlightTicketDto
    {
        public string LocalizedName { get; set; }
        public string SkyId { get; set; }
    }
}