using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Models;
using Gotorz.Server.Services;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTO;
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
    private readonly IRepository<Airport> _airportRepository;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FlightController"/> class.
    /// </summary>
    /// <param name="flightService">The <see cref="IFlightService"/> used for fetching flight-related data.</param>
    /// <param name="airportRepository">The <see cref="IRepository<Airport>"/> used to access
    /// <see cref="Airport"/> data in the database</param>
    public FlightController(IFlightService flightService, IRepository<Airport> airportRepository, IMapper mapper)
    {
        _flightService = flightService;
        _airportRepository = airportRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves names of all <see cref="Airport"/> entities in the database
    /// </summary>
    /// <returns>A collection of <see cref="Airport"/> names.</returns>
    [HttpGet("airports")]
    public IEnumerable<Airport>? GetAllAirports()
    {
        return _airportRepository.GetAll();
    }

    /// <summary>
    /// Defines an API endpoint for HTTP GET that calls <see cref="FlightService.GetAirport(string)"/>
    /// to retrieve a single matching <see cref="Airport"/>.
    /// </summary>
    /// <param name="airport">The search term to match airport names against in <see cref="FlightService.GetAirport(string)"/>.</param>
    /// <returns>An <see cref="Airport"/> entity if exactly one match is found, otherwise <c>null</c>.</returns>
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
            _airportRepository.Add(airport);
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
        // Ensure departure airport exists in database. If not, add to database with GetAirport.
        var _departureAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == departureAirport);
        if (_departureAirport == null)
        {
            var result = await GetAirportAsync(departureAirport);
            if (result is OkObjectResult okResult)
            {
                _departureAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == departureAirport);
            }
            else return new List<FlightDto>();
        }

        // Ensure arrival airport exists in database. If not, add to database with GetAirport.
        var _arrivalAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == arrivalAirport);
        if (_arrivalAirport == null)
        {
            var result = await GetAirportAsync(arrivalAirport);
            if (result is OkObjectResult okResult)
            {
                _arrivalAirport = _airportRepository.GetAll().FirstOrDefault(a => a.LocalizedName == arrivalAirport);
            }
            else return new List<FlightDto>();
        }

        // Define date
        DateOnly? _date = null;
        if (date != null) _date = DateOnly.Parse(date);

        // Retrieve flights
        if (_departureAirport != null && _arrivalAirport != null)
        {
            AirportDto _departureAirportDto = _mapper.Map<AirportDto>(_departureAirport);
            AirportDto _arrivalAirportDto = _mapper.Map<AirportDto>(_arrivalAirport);
            List<FlightDto> flights = await _flightService.GetFlightsAsync(_date, _departureAirportDto, _arrivalAirportDto);
            
            if (flights == null) return new List<FlightDto>();
            else if ( flights.Count == 0 ) return new List<FlightDto>();
            else return flights;
        }
        return new List<FlightDto>();
    }
}