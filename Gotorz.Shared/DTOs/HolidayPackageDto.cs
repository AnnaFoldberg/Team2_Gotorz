using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Gotorz.Shared.DTOs
{
    public class HolidayPackageDto
    {
        [JsonIgnore]
        public int HolidayPackageId { get; set; }
        private string tempTitle;
        public string Title
        {
            get => tempTitle;
            set
            {
                tempTitle = value;
                URL = HttpUtility.UrlEncode(value);
            }
        }
        public string Description { get; set; }
        public int MaxCapacity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal MarkupPercentage { get; set; } = 0.1M;
        public string? URL { get; set; }
    }
}