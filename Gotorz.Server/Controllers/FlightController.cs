using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTOs;
using AutoMapper;

namespace Gotorz.Server.Controllers;

/// <summary>
/// API controller for flight-related data.
/// </summary>
/// <remarks>
/// Handles incoming HTTP requests related to flight searches.
/// Delegates data retrieval to <see cref="IFlightService"/>.
/// </remarks>
/// <author>Anna</author>
[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;
    private readonly IMapper _mapper;
    private readonly IRepository<Airport> _airportRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly IRepository<FlightTicket> _flightTicketRepository;
    private readonly IRepository<HolidayPackage> _holidayPackageRepository;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FlightController"/> class.
    /// </summary>
    /// <param name="flightService">The <see cref="IFlightService"/> used for fetching flight-related data.</param>
    /// <param name="mapper">The <see cref="IMapper"/> used for mapping DTOs to Models.</param>
    /// <param name="airportRepository">The <see cref="IRepository<Airport>"/> used to access
    /// <see cref="Airport"/> data in the database.</param>
    /// /// <param name="flightRepository">The <see cref="IFlightRepository"/> used to access
    /// <see cref="Flight"/> data in the database.</param>
    /// /// <param name="flightTicketRepository">The <see cref="IRepository<FlightTicket>"/> used to access
    /// <see cref="FlightTicket"/> data in the database.</param>
    public FlightController(IFlightService flightService, IMapper mapper,
        IRepository<Airport> airportRepository, IFlightRepository flightRepository,
        IRepository<FlightTicket> flightTicketRepository, IRepository<HolidayPackage> holidayPackageRepository)
    {
        _flightService = flightService;
        _mapper = mapper;
        _airportRepository = airportRepository;
        _flightRepository = flightRepository;
        _flightTicketRepository = flightTicketRepository;
        _holidayPackageRepository = holidayPackageRepository;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that retrieves all 
    /// <see cref="Airport"/> entities from the database.
    /// </summary>
    /// <returns>A collection of <see cref="AirportDto"/> entities.</returns>
    [HttpGet("airports")]
    public async Task<IEnumerable<AirportDto>?> GetAllAirportsAsync()
    {
        var airports = await _airportRepository.GetAllAsync();
        var airportDtos = _mapper.Map<IEnumerable<AirportDto>>(airports);
        return airportDtos;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that calls <see cref="FlightService.GetAirport(string)"/>
    /// to retrieve a single matching <see cref="Airport"/>.
    /// </summary>
    /// <param name="airport">The search term to match airport names against in <see cref="FlightService.GetAirport(string)"/>.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if exactly one airport was found, otherwise <c>BadRequest</c>.</returns>
    [HttpGet("airport")]
    public async Task<IActionResult> GetAirportAsync(string airportName)
    {
        var airports = await _flightService.GetAirportAsync(airportName);

        if (airports == null) return BadRequest("Something went wrong");
        else if ( airports.Count == 0 ) return BadRequest("No airports were found");
        else if ( airports.Count > 1 ) return BadRequest("More than one airport was found");
        else
        {
            Airport airport = _mapper.Map<Airport>(airports[0]);
            await _airportRepository.AddAsync(airport);
            return Ok($"Successfully added {airport.LocalizedName} to database");
        }
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that calls <see cref="FlightService.GetFlights(string, string, string)"/>
    /// to retrieve a list of matching <see cref="FlightDto"/> entities.
    /// </summary>
    /// <param name="date">The departure date to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <param name="departureAirport">The departure airport to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <param name="arrivalAirport">The arrival airport to match the flight against in <see cref="FlightService.GetFlights(string, string, string)"/>.</param>
    /// <returns>A list of <see cref="FlightDto"/> entities matching the specified parameters.</returns>
    [HttpGet("flights")]
    public async Task<List<FlightDto>> GetFlightsAsync([FromQuery] string? date, [FromQuery] string departureAirport, [FromQuery] string arrivalAirport)
    {
        // Get all airports from database
        var airports = await _airportRepository.GetAllAsync();

        // Ensure departure airport exists in database. If not, add to database with GetAirport.
        var _departureAirport = airports.FirstOrDefault(a => a.LocalizedName == departureAirport);
        if (_departureAirport == null)
        {
            var result = await GetAirportAsync(departureAirport);
            if (result is OkObjectResult okResult)
            {
                airports = await _airportRepository.GetAllAsync();
                _departureAirport = airports.FirstOrDefault(a => a.LocalizedName == departureAirport);
            }
            else return new List<FlightDto>();
        }

        // Ensure arrival airport exists in database. If not, add to database with GetAirport.
        var _arrivalAirport = airports.FirstOrDefault(a => a.LocalizedName == arrivalAirport);
        if (_arrivalAirport == null)
        {
            var result = await GetAirportAsync(arrivalAirport);
            if (result is OkObjectResult okResult)
            {
                airports = await _airportRepository.GetAllAsync();
                _arrivalAirport = airports.FirstOrDefault(a => a.LocalizedName == arrivalAirport);
            }
            else return new List<FlightDto>();
        }

        // Define date
        DateOnly? _date = null;
        if (date != null) _date = DateOnly.Parse(date);

        // Retrieve flights
        AirportDto _departureAirportDto = _mapper.Map<AirportDto>(_departureAirport);
        AirportDto _arrivalAirportDto = _mapper.Map<AirportDto>(_arrivalAirport);
        List<FlightDto> flights = await _flightService.GetFlightsAsync(_date, _departureAirportDto, _arrivalAirportDto);
        
        if (flights == null) return new List<FlightDto>();
        else if ( flights.Count == 0 ) return new List<FlightDto>();
        else return flights;
    }

    /// <summary>
    /// Defines an API endpoint for HTTP POST that adds <see cref="FlightTicket"/> entities to the database.
    /// </summary>
    /// <param name="flightTicketsDtos">The <see cref="FlightTicketDto"/> objects representing the flight tickets to be added.</param>
    /// <returns>An <see cref="IActionResult"/> that contains <c>Ok</c> if the <see cref="FlightTicket"/> entities were
    /// added to the database successfully, otherwise <c>BadRequest</c>.</returns>
    [HttpPost("flight-tickets")]
    public async Task<IActionResult> PostFlightTicketsAsync(List<FlightTicketDto> flightTickets)
    {
        if (flightTickets == null || flightTickets.Count == 0)
        {
            return BadRequest("No flight tickets were provided");
        }

        foreach (var flightTicket in flightTickets)
        {
            // Ensure flight exists in database. If not, add to database.
            var flight = await _flightRepository.GetByFlightNumberAsync(flightTicket.Flight.FlightNumber);
            if (flight == null)
            {
                // Ensure flight contains the correct AirportIds
                var airports = await _airportRepository.GetAllAsync();
                var departureAirport = airports.FirstOrDefault(a => a.LocalizedName == flightTicket.Flight.DepartureAirport.LocalizedName);
                var arrivalAirport = airports.FirstOrDefault(a => a.LocalizedName == flightTicket.Flight.ArrivalAirport.LocalizedName);

                if (departureAirport == null || arrivalAirport == null)
                {
                    return BadRequest("One or more airports linked to flight do not exist");
                }

                flightTicket.Flight.DepartureAirport.AirportId = departureAirport.AirportId;
                flightTicket.Flight.ArrivalAirport.AirportId = arrivalAirport.AirportId;

                flight = _mapper.Map<Flight>(flightTicket.Flight);
                await _flightRepository.AddAsync(flight);
            }

            // Ensure holiday package exists in database. If not, return bad request.
            var holidayPackages = await _holidayPackageRepository.GetAllAsync();
            var holidayPackage = holidayPackages
                .FirstOrDefault(p => p.Title == flightTicket.HolidayPackage.Title &&
                p.Description == flightTicket.HolidayPackage.Description);

            if (holidayPackage == null )
            {
                return BadRequest("Holiday package linked to flight ticket does not exist");
            }

            // Add flight ticket to the database
            var _flightTicket = _mapper.Map<FlightTicket>(flightTicket);
            _flightTicket.FlightId = flight.FlightId;
            _flightTicket.HolidayPackageId = holidayPackage.HolidayPackageId;
            await _flightTicketRepository.AddAsync(_flightTicket);
        }
        return Ok($"Successfully added {flightTickets.Count()} flight ticket(s) to database");
    }
}