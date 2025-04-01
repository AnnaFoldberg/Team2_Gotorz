using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.Models
{
    /// <summary>
    /// Represents the data structure for an <see cref="Airport"/> entity.
    /// </summary>
    public class Airport
    {
        // private static int _nextId = 1;
        public int AirportId { get; set; }
        public string EntityId { get; set; }
        public string LocalizedName { get; set; }
        public string SkyId { get; set; }

        // public Airport()
        // {
        //     AirportId = _nextId++;
        //     Console.WriteLine($"Airport {LocalizedName} created with AirportId {AirportId}");
        // }
    }
}