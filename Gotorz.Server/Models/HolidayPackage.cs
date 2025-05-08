using System.ComponentModel.DataAnnotations;

namespace Gotorz.Server.Models
{
    public class HolidayPackage
    {
        [Key]
        public int HolidayPackageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // public Hotel? Hotel { get; set; }
        public int MaxCapacity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MarkupPercentage { get; set; }
    }
}

