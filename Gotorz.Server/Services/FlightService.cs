using System.Text.Json.Nodes;
using Gotorz.Shared.DTOs;

namespace Gotorz.Server.Services
{
    /// <inheritdoc />
    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for API requests.</param>
        /// <param name="config">The configuration object for accessing API keys.</param>
        public FlightService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<List<AirportDto>> GetAirportsAsync(string airportName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://move-and-rest.p.rapidapi.com/flights/auto-complete?query={airportName}"),
                Headers =
                {
                    { "x-rapidapi-key", _config.GetSection("RapidAPI:Flights:Key").Value },
                    { "x-rapidapi-host", _config.GetSection("RapidAPI:Flights:Host").Value },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var airports = new List<AirportDto>();

                var body = await response.Content.ReadAsStringAsync();
                JsonNode? root = JsonNode.Parse(body);
                JsonArray? inputSuggest = root?["inputSuggest"]?.AsArray();

                if (inputSuggest != null)
                {
                    foreach (var suggest in inputSuggest)
                    {
                        JsonObject? airportData = suggest?["navigation"]?["relevantFlightParams"]?.AsObject();

                        if (airportData == null
                            || airportData?["flightPlaceType"]?.ToString() != "AIRPORT"
                            || airportData?["localizedName"]?.ToString().Contains(airportName) == false)
                            continue;

                        string? entityId = airportData?["entityId"]?.ToString();
                        string? localizedName = airportData?["localizedName"]?.ToString();
                        string? skyId = airportData?["skyId"]?.ToString();
                        
                        airports.Add(new AirportDto { EntityId = entityId!, LocalizedName = localizedName!, SkyId = skyId! });      
                    }
                }
                return airports;
            }
        }

        /// <inheritdoc />
        public async Task<List<FlightDto>> GetFlightsAsync(DateOnly? date, AirportDto departureAirport, AirportDto arrivalAirport)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://move-and-rest.p.rapidapi.com/flights/one-way/list?origin={departureAirport.SkyId}&originId={departureAirport.EntityId}&destination={arrivalAirport.SkyId}&destinationId={arrivalAirport.EntityId}"),
                Headers =
                {
                    { "x-rapidapi-key", _config.GetSection("RapidAPI:Flights:Key").Value },
                    { "x-rapidapi-host", _config.GetSection("RapidAPI:Flights:Host").Value },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var flights = new List<FlightDto>();

                var body = await response.Content.ReadAsStringAsync();
                JsonNode? root = JsonNode.Parse(body);
                JsonArray? results = root?["data"]?["flightQuotes"]?["results"]?.AsArray();

                if (results != null)
                {
                    foreach (var result in results)
                    {
                        JsonNode? content = result?["content"];
                        if (content == null) continue;

                        bool? isDirect = content?["direct"]?.GetValue<bool>();
                        if (isDirect == null || isDirect == false) continue;
                                                
                        // Define departure date
                        string? departureDate = content?["outboundLeg"]?["localDepartureDate"]?.ToString();
                        if (departureDate == null) continue;
                        DateOnly _departureDate = DateOnly.Parse(departureDate);
                        if (date != null && _departureDate != date) continue;

                        // Define departure airport
                        AirportDto _departureAirport = departureAirport;
                        JsonNode? originAirport = content?["outboundLeg"]?["originAirport"];
                        if (originAirport == null) continue;
                        string? originEntityId = originAirport?["id"]?.ToString();
                        string? originSkyId = originAirport?["skyCode"]?.ToString();
                        if (originEntityId != departureAirport.EntityId || originSkyId != departureAirport.SkyId) continue;

                        // Define arrival airport
                        AirportDto _arrivalAirport = arrivalAirport;
                        JsonNode? destinationAirport = content?["outboundLeg"]?["destinationAirport"];
                        if (destinationAirport == null) continue;
                        string? destinationEntityId = destinationAirport?["id"]?.ToString();
                        string? destinationSkyId = destinationAirport?["skyCode"]?.ToString();

                        if (destinationEntityId != arrivalAirport.EntityId || destinationSkyId != arrivalAirport.SkyId) continue;
                        
                        // Define price for a ticket
                        double _ticketPrice = content?["rawPrice"]?.GetValue<double>() ?? 0;
                        if (_ticketPrice == 0) continue;

                        // Define id
                        string? _flightNumber = result?["id"]?.ToString();
                        if (_flightNumber == null) continue;


                        // Define flight and add to flightsw
                        flights.Add(new FlightDto { FlightNumber = _flightNumber, DepartureDate = _departureDate,
                            DepartureAirport = _departureAirport, ArrivalAirport = _arrivalAirport, TicketPrice = _ticketPrice });
                    }
                }
                return flights;
            }
        }
    } 
}