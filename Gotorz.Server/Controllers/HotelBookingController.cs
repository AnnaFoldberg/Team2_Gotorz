using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTOs;

namespace Gotorz.Server.Controllers;
[ApiController]
[Route("api/[controller]")]

public class HotelBookingController : ControllerBase
{
    private readonly IHotelBookingService _service;
    private readonly IRepository<HolidayPackage> _holidayPackageRepository;


    public HotelBookingController(IHotelBookingService service,
    IRepository<HolidayPackage> holidayPackageRepository)
    {
        _service = service;
        _holidayPackageRepository = holidayPackageRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddBooking([FromBody] HotelBookingDto bookingDto)
    {
        // Step 1: Find the holiday package based on title
        var packages = await _holidayPackageRepository.GetAllAsync();
        var holidayPackage = packages.FirstOrDefault(p =>
            p.Title.Equals(bookingDto.HolidayPackageTitle, StringComparison.OrdinalIgnoreCase));

        if (holidayPackage == null)
            return BadRequest("Holiday package not found.");

        // Step 2: Create the HotelBooking object with the correct HolidayPackageId
        var booking = new HotelBooking
        {
            HotelId = bookingDto.HotelId,
            HotelRoomId = bookingDto.HotelRoomId,
            CheckIn = bookingDto.CheckIn,
            CheckOut = bookingDto.CheckOut,
            Price = bookingDto.Price,
            RoomCapacity = bookingDto.RoomCapacity,
            HolidayPackageId = holidayPackage.HolidayPackageId
        };

        // Step 3: Save to database
        await _service.AddHotelBookingAsync(booking);
        return Ok("Hotel booking created successfully.");
    }
}