using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.Models;
using Gotorz.Server.Services;

namespace Gotorz.Server.UnitTests.Controllers
{
    [TestClass]
    public class FlightControllerTests
    {
        private FlightController _flightController;
        private Mock<ISimpleKeyRepository<Airport>> _mockAirportRepository;
        private Mock<IFlightService> _mockFlightService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAirportRepository = new Mock<ISimpleKeyRepository<Airport>>();
            _mockFlightService = new Mock<IFlightService>();

            _flightController = new FlightController(_mockFlightService.Object, _mockAirportRepository.Object);
        }

        // -------------------- GetAllAirports --------------------
        [TestMethod]
        public void GetAllAirports_SomeAirportsExist_ReturnsAllAirports()
        {

        }

        [TestMethod]
        public void GetAllAirports_SingleAirportExists_ReturnsSingleAirport()
        {

        }

        [TestMethod]
        public void GetAllAirports_NoAirportsExist_ReturnsEmptyCollection()
        {

        }

        // -------------------- GetAirport --------------------
        [TestMethod]
        public void GetAirport_SingleAirportMatching_ReturnsOk()
        {

        }

        [TestMethod]
        public void GetAirport_NoAirportsMatching_ReturnsBadRequest()
        {

        }

        [TestMethod]
        public void GetAirport_MultipleAirportsMatching_ReturnsBadRequest()
        {

        }

        [TestMethod]
        public void GetAirport_ServiceReturnsNull_ReturnsBadRequest()
        {

        }

        // -------------------- GetFlights --------------------
        [TestMethod]
        public void GetFlights_DateIsNullAndAirportsExistLocally_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_ValidDateAndAirportsExistLocally_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_InvalidDate_ThrowsFormatException()
        {

        }

        [TestMethod]
        public void GetFlights_DepartureAirportDoesNotExistLocally_CallsGetAirport()
        {

        }

        [TestMethod]
        public void GetFlights_ArrivalAirportDoesNotExistLocally_CallsGetAirport()
        {

        }

        [TestMethod]
        public void GetFlights_DepartureAirportFetchedSuccessfully_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_ArrivalAirportFetchedSuccessfully_ReturnsFlights()
        {

        }

        [TestMethod]
        public void GetFlights_FlightServiceReturnsNull_ReturnsEmptyList()
        {

        }
        
        [TestMethod]
        public void GetFlights_FlightServiceReturnsEmptyList_ReturnsEmptyList()
        {

        }
    }
}