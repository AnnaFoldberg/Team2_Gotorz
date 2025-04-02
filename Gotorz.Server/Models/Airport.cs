namespace Gotorz.Server.Models
{
    /// <summary>
    /// Represents an airport stored in the database.
    /// </summary>
    public class Airport
    {
        public int AirportId { get; set; }
        public string EntityId { get; set; }
        public string LocalizedName { get; set; }
        public string SkyId { get; set; }
    }
}