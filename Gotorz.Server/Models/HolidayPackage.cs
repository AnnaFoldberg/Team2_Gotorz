using System.Text.Json.Serialization;

namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents a holiday package stored in the database.
    /// </summary>
    public class HolidayPackage
    {
        public int HolidayPackageId { get; set; }
        public string Destination { get; set; }
        public int MaxCapacity { get; set; }
    }
}