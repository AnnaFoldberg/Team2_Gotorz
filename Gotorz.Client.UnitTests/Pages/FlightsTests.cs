using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="Flights"/> component.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class FlightsTests : Bunit.TestContext
    {
        private Mock<FlightService> _mockFlightService;
        private Mock<ILogger<Flights>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFlightService = new Mock<FlightService>();
            logger = new Mock<ILogger<Flights>>();

            Services.AddSingleton(_mockFlightService.Object);
            Services.AddSingleton(logger.Object);
        }

        // -------------------- Form --------------------
        [TestMethod]
        public void Flights_Form_MissingDepartureAirport_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();

            var arrivalAirportInput = component.Find("#arrivalAirport");
            arrivalAirportInput.Change("Los Angeles International");

            // Act
            var form = component.Find("form");
            form.Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Departure airport is required."));
        }

        [TestMethod]
        public void Flights_Form_MissingArrivalAirport_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();

            var departureAirportInput = component.Find("#departureAirport");
            departureAirportInput.Change("Los Angeles International");

            // Act
            var form = component.Find("form");
            form.Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Arrival airport is required."));
        }

        [TestMethod]
        public void Flights_Form_InvalidDateFormat_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Flights>();

            var departureAirportInput = component.Find("#departureAirport");
            departureAirportInput.Change("Los Angeles International");

            var arrivalAirportInput = component.Find("#arrivalAirport");
            arrivalAirportInput.Change("Los Angeles International");

            var dateInput = component.Find("#date");
            dateInput.Change("2025-05-05");

            // Act
            var form = component.Find("form");
            form.Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, ul.validation-errors, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count == 1);
            Assert.IsTrue(component.Markup.Contains("Date must be in the format dd-MM-yyyy."));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void Flights_OnInitializedAsync_CallsGetAllAirports()
        {

        }

        [TestMethod]
        public void Flights_OnInitializedAsync_AirportsExist_LoadsAirportsAndAirportNames()
        {

        }

        [TestMethod]
        public void Flights_OnInitializedAsync_AirportsExist_PopulatesDatalist()
        {

        }

        [TestMethod]
        public void Flights_OnInitializedAsync_NoAirportsExist_AirportsAndAirportNamesAreEmpty()
        {

        }

        [TestMethod]
        public void Flights_OnInitializedAsync_NoAirportsExist_DataListIsEmpty()
        {

        }

        // -------------------- Search --------------------
        [TestMethod]
        public void Flights_Search_ValidInput_CallsGetFlights()
        {

        }

        [TestMethod]
        public void Flights_Search_ValidInput_CallsGetAllAirports()
        {

        }

        [TestMethod]
        public void Flights_Search_FlightsFound_ShowsFlightsList()
        {

        }

        [TestMethod]
        public void Flights_Search_ServiceReturnsEmptyFlightList_ShowsNoFlightsFoundMessage()
        {

        }

        [TestMethod]
        public void Flights_Search_SearchPerformedIsTrue()
        {

        }

        [TestMethod]
        public void Flights_Search_ThrowException_LogsError()
        {

        }
    }
}