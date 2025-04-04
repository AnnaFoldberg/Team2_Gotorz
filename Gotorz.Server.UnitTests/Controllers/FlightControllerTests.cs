using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTO;
using Gotorz.Server.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Gotorz.Server.UnitTests.Controllers
{
    /// <summary>
    /// Contains unit tests for the <see cref="FlightController"/> class.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class FlightControllerTests
    {
        private FlightController _flightController;
        private Mock<IFlightService> _mockFlightService;
        private Mock<IRepository<Airport>> _mockAirportRepository;
        private Mock<IMapper> _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFlightService = new Mock<IFlightService>();
            _mockAirportRepository = new Mock<IRepository<Airport>>();
            _mapper = new Mock<IMapper>();

            _flightController = new FlightController(_mockFlightService.Object, _mockAirportRepository.Object, _mapper.Object);
        }

        // -------------------- GetAllAirports --------------------
        [TestMethod]
        public void GetAllAirports_SomeAirportsExist_ReturnsAllAirports()
        {
            // Arrange
            var mockAirports = new List<Airport>
            {
                new Airport { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" },
                new Airport { AirportId = 2, EntityId = "95673499", LocalizedName = "Dallas Fort Worth International", SkyId = "DFW" },
                new Airport { AirportId = 2, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" }
            };

            _mockAirportRepository.Setup(repo => repo.GetAll()).Returns(mockAirports);

            // Act
            var airports = _flightController.GetAllAirports();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(3, airports.Count());
            CollectionAssert.AreEqual(mockAirports, airports.ToList());
        }

        [TestMethod]
        public void GetAllAirports_SingleAirportExists_ReturnsSingleAirport()
        {
            // Arrange
            var mockAirports = new List<Airport>
            {
                new Airport { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" }
            };

            _mockAirportRepository.Setup(repo => repo.GetAll()).Returns(mockAirports);

            // Act
            var airports = _flightController.GetAllAirports();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(1, airports.Count());
            CollectionAssert.AreEqual(mockAirports, airports.ToList());
        }

        [TestMethod]
        public void GetAllAirports_NoAirportsExist_ReturnsEmptyCollection()
        {
            // Arrange
            var mockAirports = new List<Airport>();

            _mockAirportRepository.Setup(repo => repo.GetAll()).Returns(mockAirports);

            // Act
            var airports = _flightController.GetAllAirports();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count());
            CollectionAssert.AreEqual(mockAirports, airports.ToList());
        }

        // -------------------- GetAirportAsync --------------------
        [TestMethod]
        public async Task GetAirportAsync_SingleAirportMatching_ReturnsOk()
        {
            // Arrange
            string airportName = "New York John F. Kennedy";

            var airportDto = new AirportDto { AirportId = 1, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var airport = new Airport { AirportId = 1, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            _mockFlightService.Setup(s => s.GetAirportAsync(airportName))
                      .ReturnsAsync( new List<AirportDto> { airportDto } );

            _mapper.Setup(m => m.Map<Airport>(airportDto)).Returns(airport);

            // Act
            var result = await _flightController.GetAirportAsync(airportName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(result as OkObjectResult);
            Assert.AreEqual($"Successfully added {airport.LocalizedName} to database", okResult?.Value);
            Assert.IsNotNull(airport);
            _mockAirportRepository.Verify(r => r.Add(It.Is<Airport>(a => a.EntityId == airport.EntityId &&
                a.SkyId == airport.SkyId && a.LocalizedName == airport.LocalizedName)), Times.Once);
        }

        [TestMethod]
        public async Task GetAirportAsync_MultipleAirportsMatching_ReturnsBadRequest()
        {
            // Arrange
            string airportName = "London";

            var mockAirportDtos = new List<AirportDto>
            {
                new AirportDto { AirportId = 1, EntityId = "95565051", LocalizedName = "London Gatwick", SkyId = "LGW" },
                new AirportDto { AirportId = 2, EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" },
            };

            _mockFlightService.Setup(s => s.GetAirportAsync(airportName))
                .ReturnsAsync( mockAirportDtos );

            // Act
            var result = await _flightController.GetAirportAsync(airportName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(result as BadRequestObjectResult);
            Assert.AreEqual($"More than one airport was found", badRequestResult?.Value);
        }

        [TestMethod]
        public async Task GetAirportAsync_NoAirportsMatching_ReturnsBadRequest()
        {
            // Arrange
            string airportName = "Airport";

            _mockFlightService.Setup(s => s.GetAirportAsync(airportName))
                      .ReturnsAsync( new List<AirportDto>() );

            // Act
            var result = await _flightController.GetAirportAsync(airportName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(result as BadRequestObjectResult);
            Assert.AreEqual($"No airports were found", badRequestResult?.Value);
        }

        [TestMethod]
        public async Task GetAirportAsync_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            string airportName = "Airport";

            _mockFlightService.Setup(s => s.GetAirportAsync(airportName))
                      .ReturnsAsync( (List<AirportDto>?) null );

            // Act
            var result = await _flightController.GetAirportAsync(airportName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(result as BadRequestObjectResult);
            Assert.AreEqual($"Something went wrong", badRequestResult?.Value);
        }

        // -------------------- GetFlightsAsync --------------------
        [TestMethod]
        public async Task GetFlightsAsync_DateIsNullAndAirportsExistLocally_ReturnsFlights()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var airports = new List<Airport> { departureAirport, arrivalAirport};
            _mockAirportRepository.Setup(r => r.GetAll()).Returns(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            var mockFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 2),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040539*D*JFK*LHR*20250505*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 5),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                }
            };

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( mockFlights );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(3, flights.Count);
            Assert.AreEqual("{bl}:202504040539*D*JFK*LHR*20250505*airf*AF", flights[2].FlightNumber);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FlightsExistOnDateAndAirportsExistLocally_ReturnsFlights()
        {
            // Arrange
            DateOnly date = new DateOnly(2025, 5, 12);
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var airports = new List<Airport> { departureAirport, arrivalAirport};
            _mockAirportRepository.Setup(r => r.GetAll()).Returns(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            var mockFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                }
            };

            _mockFlightService.Setup(s => s.GetFlightsAsync(date, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( mockFlights );

            // Act
            var flights = await _flightController.GetFlightsAsync(date.ToString(), departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(2, flights.Count);
            Assert.AreEqual("{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI", flights[0].FlightNumber);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetFlightsAsync_AirportsExistLocallyButServiceReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var airports = new List<Airport> { departureAirport, arrivalAirport};
            _mockAirportRepository.Setup(r => r.GetAll()).Returns(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( new List<FlightDto>() );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetFlightsAsync_AirportsExistLocallyButFlightServiceReturnsNull_ReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.Setup(r => r.GetAll())
                    .Returns(new List<Airport> { departureAirport, arrivalAirport });

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( (List<FlightDto>?) null );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetFlightsAsync_DepartureAirportDoesNotExistLocallyButIsFetchedSuccessfully_CallsGetAirportAndReturnsFlights()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.SetupSequence(r => r.GetAll())
                .Returns(new List<Airport> { arrivalAirport })                    // First GetAll() when we check if departure airport exists locally
                                                                                  // It doesn't yet, therefore GetAirportAsync() is called.
                .Returns(new List<Airport> { arrivalAirport, departureAirport })  // Second GetAll() after GetAirportAsync() has been called
                                                                                  // and has added the airport to the database.
                .Returns(new List<Airport> { arrivalAirport, departureAirport }); // Third GetAll() when we check if arrival airport exists locally.

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportAsync(departureAirportName))
                      .ReturnsAsync( new List<AirportDto> { departureAirportDto } );

            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);
            _mapper.Setup(m => m.Map<Airport>(departureAirportDto)).Returns(departureAirport);
            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);

            var mockFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 2),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040539*D*JFK*LHR*20250505*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 5),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                }
            };

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( mockFlights );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(3, flights.Count);
            _mockAirportRepository.Verify(r => r.Add(It.Is<Airport>(a => a.LocalizedName == departureAirportName)), Times.Once);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(3));
            _mockFlightService.Verify(s => s.GetAirportAsync(departureAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ArrivalAirportDoesNotExistLocallyButIsFetchedSuccessfully_CallsGetAirportAndReturnsFlights()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.SetupSequence(r => r.GetAll())
                .Returns(new List<Airport> { departureAirport })                  // First GetAll() when we check if departure airport exists locally
                .Returns(new List<Airport> { departureAirport })                  // Second GetAll() when we check if arrival airport exists locally.
                                                                                  // It doesn't yet, therefore GetAirportAsync() is called.
                .Returns(new List<Airport> { departureAirport, arrivalAirport }); // Third GetAll() after GetAirportAsync() has been called
                                                                                  // and has added the airport to the database.

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportAsync(arrivalAirportName))
                      .ReturnsAsync( new List<AirportDto> { arrivalAirportDto } );

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mapper.Setup(m => m.Map<Airport>(arrivalAirportDto)).Returns(arrivalAirport);
            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            var mockFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 2),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040539*D*JFK*LHR*20250505*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 5),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto
                }
            };

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( mockFlights );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(3, flights.Count);
            _mockAirportRepository.Verify(r => r.Add(It.Is<Airport>(a => a.LocalizedName == arrivalAirportName)), Times.Once);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(3));
            _mockFlightService.Verify(s => s.GetAirportAsync(arrivalAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_DepartureAirportDoesNotExistLocallyAndIsNotFetchedSuccessfully_CallsGetAirportAndReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var airports = new List<Airport> { arrivalAirport };

            _mockAirportRepository.Setup(r => r.GetAll()).Returns( airports );

            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportAsync(departureAirportName))
                      .ReturnsAsync( new List<AirportDto>() );

            _mapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(1));
            _mockFlightService.Verify(s => s.GetAirportAsync(departureAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ArrivalAirportDoesNotExistLocallyAndIsNotFetchedSuccessfully_CallsGetAirportAndReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            var airports = new List<Airport> { departureAirport };

            _mockAirportRepository.Setup(r => r.GetAll()).Returns( airports );

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            _mockFlightService.Setup(s => s.GetAirportAsync(arrivalAirportName))
                      .ReturnsAsync( new List<AirportDto>() );

            _mapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAll(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportAsync(arrivalAirportName), Times.Once);
        }
    }
}