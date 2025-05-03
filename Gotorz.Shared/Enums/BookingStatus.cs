using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gotorz.Shared.Enums
{
    /// <summary>
    /// Represents the status options for a holiday booking
    /// </summary>
    /// <author>Anna</author>
    public enum BookingStatus
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Confirmed")]
        Confirmed,

        [Display(Name = "Cancelled")]
        Cancelled,

        [Display(Name = "Completed")]
        Completed
    }
}