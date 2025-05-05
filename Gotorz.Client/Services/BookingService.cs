using System.Net.Http.Json;
using Gotorz.Shared.DTOs;

namespace Gotorz.Client.Services
{
    /// <inheritdoc />
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for API requests.</param>
        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<string> GetNextBookingReferenceAsync()
        {
            return await _httpClient.GetStringAsync($"/Booking/booking-reference");
        }

        /// <inheritdoc />
        public async Task<HolidayBookingDto> GetHolidayBookingAsync(string bookingReference)
        {
            return await _httpClient.GetFromJsonAsync<HolidayBookingDto>($"/Booking/holiday-booking?bookingReference={bookingReference}");
        }

        /// <inheritdoc />
        public async Task PatchHolidayBookingStatusAsync(HolidayBookingDto holidayBooking)
        {
            await _httpClient.PatchAsJsonAsync($"/Booking/holiday-booking", holidayBooking);
        }

        /// <inheritdoc />
        public async Task PostHolidayBookingAsync(HolidayBookingDto holidayBooking)
        {
            await _httpClient.PostAsJsonAsync($"/Booking/holiday-booking", holidayBooking);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TravellerDto>> GetTravellersAsync(string bookingReference)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TravellerDto>>($"/Booking/travellers?bookingReference={bookingReference}");
        }

        /// <inheritdoc />
        public async Task PostTravellersAsync(List<TravellerDto> travellers)
        {
            await _httpClient.PostAsJsonAsync($"/Booking/travellers", travellers);
        }
    }
}