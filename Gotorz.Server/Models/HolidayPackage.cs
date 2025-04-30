using System.ComponentModel.DataAnnotations;

namespace Gotorz.Server.Models
{
    public class HolidayPackage
    {
        [Key]
        public int HolidayPackageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public List<Flight>? Flights { get; set; }
        // public Hotel? Hotel { get; set; }
        public int MaxCapacity { get; set; }
        public Decimal CostPrice { get; set; }
        public Decimal MarkupPercentage { get; set; }
    }
}

