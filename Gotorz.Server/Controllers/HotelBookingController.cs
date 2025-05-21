using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Shared.DTOs;

namespace Gotorz.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelBookingController : ControllerBase
{
    private readonly IHotelBookingService _service;

    // Constructor injecting the hotel booking service
    public HotelBookingController(IHotelBookingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Adds a new hotel booking to the database.
    /// </summary>
    /// <param name="bookingDto">Data transfer object containing booking details</param>
    /// <returns>HTTP 200 OK on success or HTTP 400 BadRequest on failure</returns>
    /// </remarks>
    /// <author>Sayeh</author>
    [HttpPost]
    public async Task<IActionResult> AddBooking([FromBody] HotelBookingDto bookingDto)
    {
            Console.WriteLine("📥 AddBooking called");  // TestLog

        // Validate input: booking must exist and HolidayPackageId must be set
        if (bookingDto == null || bookingDto.HolidayPackageId <= 0)
        {
            return BadRequest("Invalid booking data. HolidayPackageId is required.");
        }

        // Map DTO to domain model
        var booking = new HotelBooking
        {
            HotelId = bookingDto.HotelId,
            HotelRoomId = bookingDto.HotelRoomId,
            CheckIn = bookingDto.CheckIn,
            CheckOut = bookingDto.CheckOut,
            Price = bookingDto.Price,
            RoomCapacity = bookingDto.RoomCapacity,
            HolidayPackageId = bookingDto.HolidayPackageId.Value  // foreign key relation
        };

        // Save to database via the service
        await _service.AddHotelBookingAsync(booking);

        // Return HTTP 200 success message
        return Ok("Hotel booking created successfully.");
    }
}