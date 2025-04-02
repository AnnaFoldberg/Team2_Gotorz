using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;

namespace Gotorz.Client.UnitTests.Pages
{
    [TestClass]
    public class FlightsTests
    {
        private Mock<FlightService> _mockFlightService;
        private Mock<ILogger<Flights>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFlightService = new Mock<FlightService>();
            logger = new Mock<ILogger<Flights>>();

            // Sample data
            // ....
        }

        // -------------------- Form --------------------
        [TestMethod]
        public void Flights_Form_MissingDepartureAirport_ShowsValidationMessage()
        {

        }

        [TestMethod]
        public void Flights_Form_MissingArrivalAirport_ShowsValidationMessage()
        {

        }

        [TestMethod]
        public void Flights_Form_InvalidDateFormat_ShowsValidationMessage()
        {

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