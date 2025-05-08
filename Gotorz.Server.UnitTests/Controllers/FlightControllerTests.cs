using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
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
        private Mock<IMapper> _mockMapper;
        private Mock<IRepository<Airport>> _mockAirportRepository;
        private Mock<IFlightRepository> _mockFlightRepository;
        private Mock<IRepository<FlightTicket>> _mockFlightTicketRepository;
        private Mock<IRepository<HolidayPackage>> _mockHolidayPackageRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMapper = new Mock<IMapper>();
            _mockFlightService = new Mock<IFlightService>();
            _mockAirportRepository = new Mock<IRepository<Airport>>();
            _mockFlightRepository = new Mock<IFlightRepository>();
            _mockFlightTicketRepository = new Mock<IRepository<FlightTicket>>();
            _mockHolidayPackageRepository = new Mock<IRepository<HolidayPackage>>();

            _flightController = new FlightController(_mockMapper.Object, _mockFlightService.Object,
                _mockAirportRepository.Object, _mockFlightRepository.Object, _mockFlightTicketRepository.Object,
                _mockHolidayPackageRepository.Object);
        }

        // -------------------- GetAllAirportsAsync --------------------
        [TestMethod]
        public async Task GetAllAirportsAsync_SomeAirportsExist_ReturnsAllAirports()
        {
            // Arrange
            var mockAirports = new List<Airport>
            {
                new Airport { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" },
                new Airport { AirportId = 2, EntityId = "95673499", LocalizedName = "Dallas Fort Worth International", SkyId = "DFW" },
                new Airport { AirportId = 2, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" }
            };

            var mockAirportDtos = new List<AirportDto>
            {
                new AirportDto { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" },
                new AirportDto { AirportId = 2, EntityId = "95673499", LocalizedName = "Dallas Fort Worth International", SkyId = "DFW" },
                new AirportDto { AirportId = 2, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" }
            };

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockAirports);
            _mockMapper.Setup(m => m.Map<IEnumerable<AirportDto>>(mockAirports)).Returns(mockAirportDtos);

            // Act
            var airports = await _flightController.GetAllAirportsAsync();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(3, airports.Count());
            CollectionAssert.AreEqual(mockAirportDtos, airports.ToList());
        }

        [TestMethod]
        public async Task GetAllAirportsAsync_SingleAirportExists_ReturnsSingleAirport()
        {
            // Arrange
            var mockAirports = new List<Airport>
            {
                new Airport { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" }
            };
            var mockAirportDtos = new List<AirportDto>
            {
                new AirportDto { AirportId = 1, EntityId = "95673765", LocalizedName = "Sacramento International", SkyId = "SMF" }
            };

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockAirports);
            _mockMapper.Setup(m => m.Map<IEnumerable<AirportDto>>(mockAirports)).Returns(mockAirportDtos);

            // Act
            var airports = await _flightController.GetAllAirportsAsync();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(1, airports.Count());
            CollectionAssert.AreEqual(mockAirportDtos, airports.ToList());
        }

        [TestMethod]
        public async Task GetAllAirportsAsync_NoAirportsExist_ReturnsEmptyCollection()
        {
            // Arrange
            var mockAirports = new List<Airport>();
            var mockAirportDtos = new List<AirportDto>();

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockAirports);
            _mockMapper.Setup(m => m.Map<IEnumerable<AirportDto>>(mockAirports)).Returns(mockAirportDtos);

            // Act
            var airports = await _flightController.GetAllAirportsAsync();

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count());
            CollectionAssert.AreEqual(mockAirportDtos, airports.ToList());
        }

        // -------------------- GetAirportAsync --------------------
        [TestMethod]
        public async Task GetAirportAsync_SingleAirportMatching_ReturnsOk()
        {
            // Arrange
            string airportName = "New York John F. Kennedy";

            var airportDto = new AirportDto { AirportId = 1, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var airport = new Airport { AirportId = 1, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            _mockFlightService.Setup(s => s.GetAirportsAsync(airportName))
                      .ReturnsAsync( new List<AirportDto> { airportDto } );

            _mockMapper.Setup(m => m.Map<Airport>(airportDto)).Returns(airport);

            // Act
            var result = await _flightController.GetAirportAsync(airportName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(result as OkObjectResult);
            Assert.AreEqual($"Successfully added {airport.LocalizedName} to database", okResult?.Value);
            Assert.IsNotNull(airport);
            _mockAirportRepository.Verify(r => r.AddAsync(It.Is<Airport>(a => a.EntityId == airport.EntityId &&
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

            _mockFlightService.Setup(s => s.GetAirportsAsync(airportName))
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

            _mockFlightService.Setup(s => s.GetAirportsAsync(airportName))
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

            _mockFlightService.Setup(s => s.GetAirportsAsync(airportName))
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
            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

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
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightService.Verify(s => s.GetAirportsAsync(It.IsAny<string>()), Times.Never);
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
            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

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
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightService.Verify(s => s.GetAirportsAsync(It.IsAny<string>()), Times.Never);
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
            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( new List<FlightDto>() );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightService.Verify(s => s.GetAirportsAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetFlightsAsync_AirportsExistLocallyButFlightServiceReturnsNull_ReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(new List<Airport> { departureAirport, arrivalAirport });

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            _mockFlightService.Setup(s => s.GetFlightsAsync(null, departureAirportDto, arrivalAirportDto))
                      .ReturnsAsync( (List<FlightDto>?) null );

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_DepartureAirportDoesNotExistLocallyButIsFetchedSuccessfully_CallsGetAirportAndReturnsFlights()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.SetupSequence(r => r.GetAllAsync())
                .ReturnsAsync(new List<Airport> { arrivalAirport })                    // First GetAllAsync() when only arrival airport exists.
                .ReturnsAsync(new List<Airport> { arrivalAirport, departureAirport }); // Second GetAllAsync() after GetAirportAsync() has been called
                                                                                       // and has added the departure airport to the database.

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportsAsync(departureAirportName))
                    .ReturnsAsync( new List<AirportDto> { departureAirportDto } );

            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);
            _mockMapper.Setup(m => m.Map<Airport>(departureAirportDto)).Returns(departureAirport);
            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);

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
                    .ReturnsAsync(mockFlights);

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(3, flights.Count);
            _mockAirportRepository.Verify(r => r.AddAsync(It.Is<Airport>(a => a.LocalizedName == departureAirportName)), Times.Once);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportsAsync(departureAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ArrivalAirportDoesNotExistLocallyButIsFetchedSuccessfully_CallsGetAirportAndReturnsFlights()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.SetupSequence(r => r.GetAllAsync())
                .ReturnsAsync(new List<Airport> { departureAirport })                  // First GetAllAsync() when only departure airport exists.
                .ReturnsAsync(new List<Airport> { departureAirport, arrivalAirport }); // Second GetAllAsync() after GetAirportAsync() has been called
                                                                                       // and has added the arrival airport to the database.

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportsAsync(arrivalAirportName))
                      .ReturnsAsync( new List<AirportDto> { arrivalAirportDto } );

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<Airport>(arrivalAirportDto)).Returns(arrivalAirport);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

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
            _mockAirportRepository.Verify(r => r.AddAsync(It.Is<Airport>(a => a.LocalizedName == arrivalAirportName)), Times.Once);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Exactly(2));
            _mockFlightService.Verify(s => s.GetAirportsAsync(arrivalAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_DepartureAirportDoesNotExistLocallyAndIsNotFetchedSuccessfully_CallsGetAirportAndReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var arrivalAirport = new Airport { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            var airports = new List<Airport> { arrivalAirport };

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockFlightService.Setup(s => s.GetAirportsAsync(departureAirportName))
                      .ReturnsAsync( new List<AirportDto>() );

            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightService.Verify(s => s.GetAirportsAsync(departureAirportName), Times.Once);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ArrivalAirportDoesNotExistLocallyAndIsNotFetchedSuccessfully_CallsGetAirportAndReturnsEmptyList()
        {
            // Arrange
            string departureAirportName = "New York John F. Kennedy";
            string arrivalAirportName = "London Heathrow";

            var departureAirport = new Airport { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            var airports = new List<Airport> { departureAirport };

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };

            _mockFlightService.Setup(s => s.GetAirportsAsync(arrivalAirportName))
                      .ReturnsAsync( new List<AirportDto>() );

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);

            // Act
            var flights = await _flightController.GetFlightsAsync(null, departureAirportName, arrivalAirportName);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightService.Verify(s => s.GetAirportsAsync(arrivalAirportName), Times.Once);
        }

        // -------------------- PostFlightTicketsAsync --------------------
        [TestMethod]
        public async Task PostFlightTicketsAsync_MultipleIdenticalFlightTickets_AddsFlightTickets()
        {
            // Arrange
            // Flights
            var mockFlightDto = new FlightDto { FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12), TicketPrice = 110.0 };

            var mockFlight = new Flight { FlightId = 1, FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12)};

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDto)).Returns(mockFlight);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlight.FlightNumber)).ReturnsAsync(mockFlight);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 100.0,
                    Flight = mockFlight,
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlight,
                    HolidayPackage = mockHolidayPackage
                },
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 2 flight ticket(s) to database", okResult.Value);
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_FlightTicketsHaveDifferentFlights_AddsFlightTickets()
        {
            // Arrange
            // Flights
            var mockFlightDtos = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    TicketPrice = 110.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    TicketPrice = 100.0
                }
            };

            var mockFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 1,
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                },
                new Flight
                {
                    FlightId = 2,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                }
            };

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[0])).Returns(mockFlights[0]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[1])).Returns(mockFlights[1]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[0].FlightNumber)).ReturnsAsync(mockFlights[0]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[1].FlightNumber)).ReturnsAsync(mockFlights[1]);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDtos[0],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDtos[1],
                    HolidayPackage = mockHolidayPackageDto
                }
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlights[0],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[1],
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 2 flight ticket(s) to database", okResult.Value);
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_FlightTicketsIsEmptyList_ReturnsBadRequest()
        {
            // Arrange
            var mockFlightTicketDtos = new List<FlightTicketDto>();

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No flight tickets were provided", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_FlightTicketsIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _flightController.PostFlightTicketsAsync(null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No flight tickets were provided", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_AllOfMultipleFlightsExistLocally_AddsFlightTickets()
        {
            // Arrange
            // Flights
            var mockFlightDtos = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    TicketPrice = 110.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    TicketPrice = 100.0
                }
            };

            var mockFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 1,
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                },
                new Flight
                {
                    FlightId = 2,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                }
            };

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[0])).Returns(mockFlights[0]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[1])).Returns(mockFlights[1]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[0].FlightNumber)).ReturnsAsync(mockFlights[0]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[1].FlightNumber)).ReturnsAsync(mockFlights[1]);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDtos[0],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDtos[1],
                    HolidayPackage = mockHolidayPackageDto
                }
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlights[0],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[1],
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 2 flight ticket(s) to database", okResult.Value);
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_SomeOfMultipleFlightsExistLocally_AddsMissingFlightsAndFlightTickets()
        {
            // Arrange
            // Airports
            var departureAirport = new Airport { AirportId = 21, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { AirportId = 22, EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };
            var airports = new List<Airport> { departureAirport, arrivalAirport};

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);
            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            // Flights
            var mockFlightDtos = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 110.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 100.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 105.0
                }
            };

            var mockFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 1,
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 2,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 3,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                }
            };

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[0])).Returns(mockFlights[0]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[1])).Returns(mockFlights[1]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[2])).Returns(mockFlights[2]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[0].FlightNumber)).ReturnsAsync(mockFlights[0]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[1].FlightNumber)).ReturnsAsync(mockFlights[1]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[2].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.AddAsync(It.IsAny<Flight>())).Returns(Task.CompletedTask);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDtos[0],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDtos[1],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 105.0,
                    Flight = mockFlightDtos[2],
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlights[0],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[1],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[2],
                    HolidayPackage = mockHolidayPackage
                },
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[2])).Returns(mockFlightTickets[2]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 3 flight ticket(s) to database", okResult.Value);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockHolidayPackageRepository.Verify(r => r.GetAllAsync(), Times.Exactly(3));
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(3));
            _mockFlightRepository.Verify(s => s.AddAsync(It.IsAny<Flight>()), Times.Once);
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_SingleAndOnlyFlightExistsLocally_AddsFlightTickets()
        {
            // Arrange
            // Flights
            var mockFlightDto = new FlightDto { FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12), TicketPrice = 110.0 };

            var mockFlight = new Flight { FlightId = 1, FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12)};

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDto)).Returns(mockFlight);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlight.FlightNumber)).ReturnsAsync(mockFlight);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlight,
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlight,
                    HolidayPackage = mockHolidayPackage
                },
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 2 flight ticket(s) to database", okResult.Value);
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_NoneOfMultipleFlightsExistLocally_AddsMissingFlightsAndFlightTickets()
        {
            // Arrange
            // Airports
            var departureAirport = new Airport { AirportId = 21, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { AirportId = 22, EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };
            var airports = new List<Airport> { departureAirport, arrivalAirport};

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockMapper.Setup(m => m.Map<AirportDto>(departureAirport)).Returns(departureAirportDto);
            _mockMapper.Setup(m => m.Map<AirportDto>(arrivalAirport)).Returns(arrivalAirportDto);
            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);

            // Flights
            var mockFlightDtos = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 110.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 100.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 105.0
                }
            };

            var mockFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 1,
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 2,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 3,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                }
            };

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[0])).Returns(mockFlights[0]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[1])).Returns(mockFlights[1]);
            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDtos[2])).Returns(mockFlights[2]);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[0].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[1].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[2].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.AddAsync(It.IsAny<Flight>())).Returns(Task.CompletedTask);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDtos[0],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDtos[1],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 105.0,
                    Flight = mockFlightDtos[2],
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlights[0],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[1],
                    HolidayPackage = mockHolidayPackage
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlights[2],
                    HolidayPackage = mockHolidayPackage
                },
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[2])).Returns(mockFlightTickets[2]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 3 flight ticket(s) to database", okResult.Value);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Exactly(3));
            _mockHolidayPackageRepository.Verify(r => r.GetAllAsync(), Times.Exactly(3));
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Exactly(3));
            _mockFlightRepository.Verify(s => s.AddAsync(It.IsAny<Flight>()), Times.Exactly(3));
            _mockFlightTicketRepository.Verify(s => s.AddAsync(It.IsAny<FlightTicket>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_AirportsLinkedToFlightDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            // Airports
            var departureAirport = new Airport { AirportId = 21, EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirport = new Airport { AirportId = 22, EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };
            var airports = new List<Airport> { departureAirport, arrivalAirport};

            var departureAirportDto = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            var arrivalAirportDto = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            _mockAirportRepository.Setup(r => r.GetAllAsync()).ReturnsAsync( new List<Airport>() );

            // Flights
            var mockFlightDtos = new List<FlightDto>
            {
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 110.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 100.0
                },
                new FlightDto
                {
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirport = departureAirportDto,
                    ArrivalAirport = arrivalAirportDto,
                    TicketPrice = 105.0
                }
            };

            var mockFlights = new List<Flight>
            {
                new Flight
                {
                    FlightId = 1,
                    FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 2,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*airf*AF",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                },
                new Flight
                {
                    FlightId = 3,
                    FlightNumber = "{bl}:202504040246*D*JFK*LHR*20250502*smrf*AI",
                    DepartureDate = new DateOnly(2025, 5, 12),
                    DepartureAirportId = departureAirport.AirportId,
                    ArrivalAirportId = arrivalAirport.AirportId,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport
                }
            };

            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[0].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[1].FlightNumber)).ReturnsAsync((Flight)null!);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlights[2].FlightNumber)).ReturnsAsync((Flight)null!);

            // HolidayPackages
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });

            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDtos[0],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDtos[1],
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 105.0,
                    Flight = mockFlightDtos[2],
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"One or more airports linked to flight do not exist", badRequestResult.Value);
            _mockAirportRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockFlightRepository.Verify(s => s.GetByFlightNumberAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task PostFlightTicketsAsync_HolidayPackageLinkedToFlightTicketDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            // Flights
            var mockFlightDto = new FlightDto { FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12), TicketPrice = 110.0 };

            var mockFlight = new Flight { FlightId = 1, FlightNumber = "{bl}:202504040709*D*JFK*LHR*20250512*smtf*FI",
                    DepartureDate = new DateOnly(2025, 5, 12)};

            _mockMapper.Setup(m => m.Map<Flight>(mockFlightDto)).Returns(mockFlight);
            _mockFlightRepository.Setup(r => r.GetByFlightNumberAsync(mockFlight.FlightNumber)).ReturnsAsync(mockFlight);

            // HolidayPackages
            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage>());

            // Flight tickets
            // Flight tickets
            var mockFlightTicketDtos = new List<FlightTicketDto>
            {
                new FlightTicketDto
                {
                    Price = 110.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
                new FlightTicketDto
                {
                    Price = 100.0,
                    Flight = mockFlightDto,
                    HolidayPackage = mockHolidayPackageDto
                },
            };

            var mockFlightTickets = new List<FlightTicket>
            {
                new FlightTicket
                {
                    FlightTicketId = 11,
                    Price = 110.0,
                    Flight = mockFlight
                },
                new FlightTicket
                {
                    FlightTicketId = 12,
                    Price = 100.0,
                    Flight = mockFlight
                },
            };

            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[0])).Returns(mockFlightTickets[0]);
            _mockMapper.Setup(m => m.Map<FlightTicket>(mockFlightTicketDtos[1])).Returns(mockFlightTickets[1]);
            _mockFlightTicketRepository.Setup(r => r.AddAsync(It.IsAny<FlightTicket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _flightController.PostFlightTicketsAsync(mockFlightTicketDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"Holiday package linked to flight ticket does not exist", badRequestResult.Value);
            _mockHolidayPackageRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}