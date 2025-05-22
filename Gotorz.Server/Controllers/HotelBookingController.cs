using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Shared.DTOs;
using Gotorz.Server.DataAccess;
using AutoMapper;



namespace Gotorz.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelBookingController : ControllerBase
{
    private readonly IHotelBookingService _service;
    private readonly IHolidayPackageRepository _holidayPackageRepository;
    private readonly IHotelBookingRepository _hotelBookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IHotelRoomRepository _hotelRoomRepository;

    private readonly IMapper _mapper;

    // Constructor injecting the hotel booking service
    public HotelBookingController(IHotelBookingService service, IHolidayPackageRepository holidayPackageRepository, IHotelBookingRepository hotelBookingRepository, IHotelRepository hotelRepository, IHotelRoomRepository hotelRoomRepository, IMapper mapper)
    {
        _service = service;
        _holidayPackageRepository = holidayPackageRepository;
        _hotelBookingRepository = hotelBookingRepository;
        _hotelRepository = hotelRepository;
        _hotelRoomRepository = hotelRoomRepository;
        _mapper = mapper;
        
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
        // Validate input: booking must exist and HolidayPackage must be set
        if (bookingDto == null || bookingDto.HolidayPackageDto == null || bookingDto.HotelRoom == null)
    {
        return BadRequest("Booking data mangler.");
    }

        var booking = _mapper.Map<HotelBooking>(bookingDto);

        var hotelRoom = (await _hotelRoomRepository.GetAllAsync()).FirstOrDefault(r => r.ExternalRoomId == bookingDto.HotelRoom.ExternalRoomId);
        
        if (hotelRoom == null)
        {
            return BadRequest();
        }
        booking.HotelRoomId = hotelRoom.HotelRoomId;
        var holidayPackage = await _holidayPackageRepository.GetByUrlAsync(bookingDto.HolidayPackageDto.URL);
        if (holidayPackage == null)
        {
            return BadRequest();
        }
        booking.HolidayPackageId = holidayPackage.HolidayPackageId;

        // Save to database via the service
        await _service.AddHotelBookingAsync(booking);

        return Ok("Hotel booking created successfully.");
    }
}