using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Services;
using Moq.Protected;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Gotorz.Server.UnitTests.Services
{
    [TestClass]
    public class FlightServiceTests
    {
        private FlightService _flightService;
        private Mock<IConfiguration> _mockConfig;
        private HttpClient _httpClient;
        
        [TestInitialize]
        public void TestInitialize()
        {
            // _mockHttpHandler and _mockConfig based on a ChatGPT-generated template.
            // Customized for this project.
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("fakeJson")
            });
            _httpClient = new HttpClient(mockHttpHandler.Object);
            
            var keySection = new Mock<IConfigurationSection>();
            keySection.Setup(s => s.Value).Returns("fake-api-key-1234567890abcdef");

            var hostSection = new Mock<IConfigurationSection>();
            hostSection.Setup(s => s.Value).Returns("fake-api.rapidapi.test");

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c.GetSection("RapidAPI:Key")).Returns(keySection.Object);
            _mockConfig.Setup(c => c.GetSection("RapidAPI:Host")).Returns(hostSection.Object);

            _flightService = new FlightService(_httpClient, _mockConfig.Object);
        }

        // -------------------- GetAirport --------------------
        [TestMethod]
        public void GetAirport_FoundMatchingAirports_ReturnsAirports()
        {

        }

        [TestMethod]
        public void GetAirport_FoundSingleMatchingAirport_ReturnsAirports()
        {

        }

        [TestMethod]
        public void GetAirport_FoundNoMatchingAirport_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetAirport_InputSuggestNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetAirport_NavigationNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetAirport_RelevantFlightParamsNodeMissing_ReturnsEmptyList()
        {

        }

        // -------------------- GetFlights --------------------
        [TestMethod]
        public void GetFlights_DateIsNullAndFlightsExist_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_ValidDateAndFlightsExist_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_SingleFlightExists_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_NoFlightsExistBetweenAirports_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_NoFlightsExistOnDate_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_InvalidDate_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FetchedDateDoesNotMatchSpecifiedDate_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FetchedOriginAirportIdDoesNotMatchSpecifiedDepartureAirportEntityId_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FetchedOriginAirportSkyCodeDoesNotMatchSpecifiedDepartureAirportSkyId_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FetchedDestinationAirportIdDoesNotMatchSpecifiedArrivalAirportEntityId_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FetchedDestinationAirportSkyCodeDoesNotMatchSpecifiedArrivalAirportSkyId_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FlightIsNotDirect_ReturnsEmptyList()
        {

        }
        
        [TestMethod]
        public void GetFlights_DataNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_FlightQuotesNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_ResultsNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_ContentNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_DirectNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_OutboundNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_OriginAirportNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_DestinationAirportNodeMissing_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void GetFlights_IdNodeMissing_ReturnsEmptyList()
        {

        }
    }
}