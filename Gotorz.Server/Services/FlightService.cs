using System.Text.Json.Nodes;
using Gotorz.Shared.DTO;

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
        public async Task<List<AirportDto>> GetAirportAsync(string airportName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://skyscanner89.p.rapidapi.com/flights/auto-complete?query={airportName}"),
                Headers =
                {
                    { "x-rapidapi-key", _config.GetSection("RapidAPI:Key").Value },
                    { "x-rapidapi-host", _config.GetSection("RapidAPI:Host").Value },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var airports = new List<AirportDto>();

                var body = await response.Content.ReadAsStringAsync();
                JsonNode? root = JsonNode.Parse(body);
                JsonObject? airportData = root?["inputSuggest"]?[0]?["navigation"]?["relevantFlightParams"]?.AsObject();

                if (airportData != null)
                {
                    string? entityId = airportData?["entityId"]?.ToString();
                    string? localizedName = airportData?["localizedName"]?.ToString();
                    string? skyId = airportData?["skyId"]?.ToString();

                    airports.Add(new AirportDto { EntityId = entityId, LocalizedName = localizedName, SkyId = skyId });
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
                RequestUri = new Uri($"https://skyscanner89.p.rapidapi.com/flights/one-way/list?origin={departureAirport.SkyId}&originId={departureAirport.EntityId}&destination={arrivalAirport.SkyId}&destinationId={arrivalAirport.EntityId}"),
                Headers =
                {
                    { "x-rapidapi-key", _config.GetSection("RapidAPI:Key").Value },
                    { "x-rapidapi-host", _config.GetSection("RapidAPI:Host").Value },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                var flights = new List<FlightDto>();

                var body = await response.Content.ReadAsStringAsync();
                JsonNode root = JsonNode.Parse(body);
                JsonArray? results = root?["data"]?["flightQuotes"]?["results"]?.AsArray();

                if (results != null)
                {
                    foreach (var result in results)
                    {
                        JsonNode? content = result?["content"];
                        if (content == null || content?["direct"]?.GetValue<bool>() == false) continue;
                                                
                        // Define departure date
                        string departureDate = content?["outboundLeg"]?["localDepartureDate"].ToString();
                        DateOnly _departureDate = DateOnly.Parse(departureDate);
                        if (date != null && _departureDate != date) continue;

                        // Define departure airport
                        AirportDto _departureAirport = departureAirport;
                        JsonNode originAirport = content?["outboundLeg"]?["originAirport"];
                        if (originAirport != null)
                        {
                            string? entityId = originAirport?["id"]?.ToString();
                            string? skyId = originAirport?["skyCode"]?.ToString();

                            if (entityId != departureAirport.EntityId || skyId != departureAirport.SkyId) continue;
                        }

                        // Define arrival airport
                        AirportDto _arrivalAirport = arrivalAirport;
                        JsonNode destinationAirport = content?["outboundLeg"]?["destinationAirport"];
                        if (destinationAirport != null)
                        {
                            string? entityId = destinationAirport?["id"]?.ToString();
                            string? skyId = destinationAirport?["skyCode"]?.ToString();

                            if (entityId != arrivalAirport.EntityId || skyId != arrivalAirport.SkyId) continue;
                        }
                        
                        // Define id
                        string _flightNumber = result?["id"]?.ToString();

                        // Define flight and add to flights
                        flights.Add(new FlightDto { FlightNumber = _flightNumber, DepartureDate = _departureDate, DepartureAirport = _departureAirport, ArrivalAirport = _arrivalAirport });
                    }
                }
                return flights;
            }
        }
    } 
}