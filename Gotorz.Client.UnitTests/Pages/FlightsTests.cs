using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Gotorz.Shared.DTO;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="Flights"/> component.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class FlightsTests : Bunit.TestContext
    {
        private Mock<IFlightService> _mockFlightService;
        private Mock<ILogger<Flights>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFlightService = new Mock<IFlightService>();
            logger = new Mock<ILogger<Flights>>();

            Services.AddSingleton(_mockFlightService.Object);
            Services.AddSingleton(logger.Object);
        }

        // -------------------- Form --------------------
        [TestMethod]
        public void Form_MissingDepartureAirport_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();
            component.Find("#arrivalAirport").Change("Los Angeles International");

            // Act
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Departure airport is required."));
        }

        [TestMethod]
        public void Form_MissingArrivalAirport_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();
            component.Find("#departureAirport").Change("Los Angeles International");

            // Act
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Arrival airport is required."));
        }

        [TestMethod]
        public void Form_InvalidDateFormat_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();
            component.Find("#departureAirport").Change("Los Angeles International");
            component.Find("#arrivalAirport").Change("New York John F. Kennedy");
            component.Find("#date").Change("2025-05-05");

            // Act
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Date must be in the format dd-MM-yyyy"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_CallsGetAllAirports()
        {
            // Act
            var component = RenderFlightsWithAirports();

            // Assert
            _mockFlightService.Verify(s => s.GetAllAirportsAsync(), Times.Once());
        }

        [TestMethod]
        public void OnInitializedAsync_LoadsAirports()
        {
            // Act
            var component = RenderFlightsWithAirports();

            // Assert
            Assert.IsTrue(component.Markup.Contains("New York John F. Kennedy"));
            Assert.IsTrue(component.Markup.Contains("London Heathrow"));
        }

        [TestMethod]
        public void OnInitializedAsync_AirportsExist_PopulatesDatalist()
        {
            // Act
            var component = RenderFlightsWithAirports();

            // Assert
            var departureDatalist = component.Find("datalist#departure-options");
            var arrivalDatalist = component.Find("datalist#arrival-options");
            var departureOptions = departureDatalist.GetElementsByTagName("option");
            var arrivalOptions = arrivalDatalist.GetElementsByTagName("option");
            
            Assert.AreEqual(2, departureOptions.Length);
            Assert.AreEqual(2, arrivalOptions.Length);
            Assert.IsTrue(departureOptions.Any(o => o.GetAttribute("value") == "New York John F. Kennedy"));
            Assert.IsTrue(departureOptions.Any(o => o.GetAttribute("value") == "London Heathrow"));
            Assert.IsTrue(arrivalOptions.Any(o => o.GetAttribute("value") == "New York John F. Kennedy"));
            Assert.IsTrue(arrivalOptions.Any(o => o.GetAttribute("value") == "London Heathrow"));
        }

        [TestMethod]
        public void OnInitializedAsync_NoAirportsExist_DataListIsEmpty()
        {
            // Arrange
            _mockFlightService.Setup(s => s.GetAllAirportsAsync()).ReturnsAsync( new List<AirportDto>() );

            // Act
            var component = RenderComponent<Flights>();

            // Assert
            _mockFlightService.Verify(s => s.GetAllAirportsAsync(), Times.Once());
            var datalistOptions = component.FindAll("datalist option");
            Assert.AreEqual(0, datalistOptions.Count);
        }

        // -------------------- SearchAsync --------------------
        [TestMethod]
        public void SearchAsync_ValidInput_CallsGetFlights()
        {
            // Arrange
            var (mockFlights, component) = RenderFlightsWithFlights();

            component.Find("#departureAirport").Change("New York John F. Kennedy");
            component.Find("#arrivalAirport").Change("London Heathrow");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockFlightService.Verify(s => s
                .GetFlightsAsync(null, "New York John F. Kennedy", "London Heathrow"), Times.Once());
        }

        [TestMethod]
        public void SearchAsync_ValidInput_LoadsFlights()
        {
            // Arrange
            var (mockFlights, component) = RenderFlightsWithFlights();

            component.Find("#departureAirport").Change("New York John F. Kennedy");
            component.Find("#arrivalAirport").Change("London Heathrow");

            // Act
            component.Find("form").Submit();

            // Assert
            foreach (var flight in mockFlights)
            {
                Assert.IsTrue(component.Markup.Contains(flight.DepartureDate.ToString("dd-MM-yyyy")));
                Assert.IsTrue(component.Markup.Contains(flight.DepartureAirport.LocalizedName));
                Assert.IsTrue(component.Markup.Contains(flight.ArrivalAirport.LocalizedName));
            }
        }

        [TestMethod]
        public void SearchAsync_ValidInput_CallsGetAllAirports()
        {
            // Arrange
            var (mockFlights, component) = RenderFlightsWithFlights();

            component.Find("#departureAirport").Change("New York John F. Kennedy");
            component.Find("#arrivalAirport").Change("London Heathrow");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockFlightService.Verify(s => s.GetAllAirportsAsync(), Times.Exactly(2));
        }

        [TestMethod]
        public void SearchAsync_ServiceReturnsEmptyFlightList_ShowsNoFlightsFoundMessage()
        {
            // Arrange
            _mockFlightService.Setup(s => s.GetFlightsAsync(null, "New York John F. Kennedy", "London Heathrow"))
                      .ReturnsAsync( new List<FlightDto>() );

            var component = RenderComponent<Flights>();

            component.Find("#departureAirport").Change("New York John F. Kennedy");
            component.Find("#arrivalAirport").Change("London Heathrow");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("No flights were found"));
        }

        [TestMethod]
        public void SearchAsync_ThrowException_LogsError()
        {
            // Arrange
            _mockFlightService.Setup(s => s.GetFlightsAsync(null, "New York John F. Kennedy", "London Heathrow"))
                      .ThrowsAsync(new Exception("Mocked flight service failure"));

            var component = RenderComponent<Flights>();

            component.Find("#departureAirport").Change("New York John F. Kennedy");
            component.Find("#arrivalAirport").Change("London Heathrow");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsFalse(component.Markup.Contains("No flights were found"));
            Assert.IsFalse(component.Markup.Contains("<ul>"));
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error fetching flights")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // -------------------- Helper Methods --------------------
        private IRenderedComponent<Flights> RenderFlightsWithAirports()
        {
            var mockAirports = new List<AirportDto>
            { 
                new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" },
                new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" }
            };

            _mockFlightService.Setup(s => s.GetAllAirportsAsync()).ReturnsAsync(mockAirports);

            return RenderComponent<Flights>();
        }

        private (List<FlightDto>, IRenderedComponent<Flights>) RenderFlightsWithFlights()
        {
            var departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var mockFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 2),
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040539*D*JFK*LHR*20250505*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 5),
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                }
            };

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirport.LocalizedName, arrivalAirport.LocalizedName))
                      .ReturnsAsync( mockFlights );

            var component = RenderComponent<Flights>();
            
            return (mockFlights, component);
        }
    }
}