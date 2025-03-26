using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Models;
using Gotorz.Server.Services;

namespace Gotorz.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private IFlightService _flightService;
    private Airport _airport;
    
    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet("airport")]
    public async Task<Airport> GetAirport(string airport)
    {
        var airports = await _flightService.GetAirport(airport);

        if (airports == null) return null;
        else if ( airports.Count == 0 ) return null;
        else if ( airports.Count > 1 ) return null;
        else return airports[0];
    }

    [HttpGet("flights")]
    public async Task<List<Flight>> GetFlights([FromQuery] string date, [FromQuery] string departureAirport, [FromQuery] string arrivalAirport)
    {
        // Hvis den findes i databasen, hent Airport-objekt derfra, noget med airports.GetAll fra et repository
        // De Airports, der findes i vores database, er dem, der skal dukke op ved drop-down i departure- og arrivalfelterne på hjemmesiden
        // Hvis ikke, brug GetAirport() som nedenfor
    
        var _departureAirport = await GetAirport(departureAirport);
        var _arrivalAirport = await GetAirport(arrivalAirport);

        DateOnly _date = DateOnly.Parse(date);

        if (_departureAirport != null && _arrivalAirport != null)
        {
            List<Flight> flights = await _flightService.GetFlights(_date, _departureAirport, _arrivalAirport);
            
            if (flights == null) return null;
            else if ( flights.Count == 0 ) return null;
            else return flights;
        }
        return null;
    }
}