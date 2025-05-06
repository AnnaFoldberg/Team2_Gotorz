using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gotorz.Shared.DTOs
{
    public class HolidayPackageDto
    {
        [JsonIgnore]
        public int HolidayPackageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxCapacity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MarkupPercentage { get; set; } = 0.1M;
    }
}