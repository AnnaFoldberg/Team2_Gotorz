using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Services;
using Moq.Protected;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text;
using Gotorz.Shared.DTO;

namespace Gotorz.Server.UnitTests.Services
{
    /// <summary>
    /// Contains unit tests for the <see cref="FlightService"/> class.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class FlightServiceTests
    {
        private FlightService _flightService;
        private Mock<IConfiguration> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _httpClient;
        
        [TestInitialize]
        public void TestInitialize()
        {
            // _mockHttpHandler and _mockConfig based on a ChatGPT-generated template.
            // Customized for this project.
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object);
            
            var keySection = new Mock<IConfigurationSection>();
            keySection.Setup(s => s.Value).Returns("fake-api-key-1234567890abcdef");

            var hostSection = new Mock<IConfigurationSection>();
            hostSection.Setup(s => s.Value).Returns("fake-api.rapidapi.test");

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c.GetSection("RapidAPI:Key")).Returns(keySection.Object);
            _mockConfig.Setup(c => c.GetSection("RapidAPI:Host")).Returns(hostSection.Object);
        }

        // -------------------- GetAirportAsync --------------------
        [TestMethod]
        public async Task GetAirportAsync_FoundMatchingAirports_ReturnsAirports()
        {
            // Arrange
            string json = @"
                {
                ""inputSuggest"": [
                    {
                        ""navigation"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""27544008"",
                                ""flightPlaceType"": ""CITY"",
                                ""localizedName"": ""London"",
                                ""skyId"": ""LOND""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565051"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Gatwick"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565051"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Gatwick"",
                                ""skyId"": ""LGW""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565050"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Heathrow"",
                                ""skyId"": ""LHR""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565052"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Stansted"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565052"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Stansted"",
                                ""skyId"": ""STN""
                            }
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("London");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(3, airports.Count);
            Assert.AreEqual("LGW", airports[0].SkyId);
            Assert.AreEqual("LHR", airports[1].SkyId);
            Assert.AreEqual("STN", airports[2].SkyId);
        }

        [TestMethod]
        public async Task GetAirportAsync_FoundSingleMatchingAirport_ReturnsAirports()
        {
            // Arrange
            string json = @"
                {
                ""inputSuggest"": [
                    {
                        ""navigation"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""27544008"",
                                ""flightPlaceType"": ""CITY"",
                                ""localizedName"": ""London"",
                                ""skyId"": ""LOND""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565050"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Heathrow"",
                                ""skyId"": ""LHR""
                            }
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("London Heathrow");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(1, airports.Count);
            Assert.AreEqual("LHR", airports[0].SkyId);
        }

        [TestMethod]
        public async Task GetAirportAsync_FoundNoMatchingAirport_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                ""inputSuggest"": [
                    {
                        ""navigation"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""27544008"",
                                ""flightPlaceType"": ""CITY"",
                                ""localizedName"": ""London"",
                                ""skyId"": ""LOND""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565050"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Heathrow"",
                                ""skyId"": ""LHR""
                            }
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("Rome Fiumicino");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count);
        }

        [TestMethod]
        public async Task GetAirportAsync_InputSuggestNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                ""missing"": [
                    {
                        ""navigation"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""27544008"",
                                ""flightPlaceType"": ""CITY"",
                                ""localizedName"": ""London"",
                                ""skyId"": ""LOND""
                            }
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565050"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Heathrow"",
                                ""skyId"": ""LHR""
                            }
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("London Heathrow");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count);
        }

        [TestMethod]
        public async Task GetAirportAsync_NavigationNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                ""inputSuggest"": [
                    {
                        ""missing"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""27544008"",
                                ""flightPlaceType"": ""CITY"",
                                ""localizedName"": ""London"",
                                ""skyId"": ""LOND""
                            }
                        }
                    },
                    {
                        ""missing"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""relevantFlightParams"":
                            {
                                ""entityId"": ""95565050"",
                                ""flightPlaceType"": ""AIRPORT"",
                                ""localizedName"": ""London Heathrow"",
                                ""skyId"": ""LHR""
                            }
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("London Heathrow");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count);
        }

        [TestMethod]
        public async Task GetAirportAsync_RelevantFlightParamsNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                ""inputSuggest"": [
                    {
                        ""navigation"": {
                            ""entityId"": ""27544008"",
                            ""entityType"": ""CITY"",
                            ""flightPlaceType"": ""CITY"",
                            ""localizedName"": ""London"",
                            ""skyId"": ""LOND""
                        }
                    },
                    {
                        ""navigation"": {
                            ""entityId"": ""95565050"",
                            ""entityType"": ""AIRPORT"",
                            ""flightPlaceType"": ""AIRPORT"",
                            ""localizedName"": ""London Heathrow"",
                            ""skyId"": ""LHR""
                        }
                    }
                ]
            }";
            Setup(json);

            // Act
            var airports = await _flightService.GetAirportAsync("London Heathrow");

            // Assert
            Assert.IsNotNull(airports);
            Assert.AreEqual(0, airports.Count);
        }

        // -------------------- GetFlightsAsync --------------------
        [TestMethod]
        public async Task GetFlightsAsync_DateIsNullAndFlightsExist_ReturnsFlights()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(2, flights.Count);
            Assert.AreSame(departureAirport, flights[0].DepartureAirport);
            Assert.AreSame(arrivalAirport, flights[0].ArrivalAirport);
            Assert.AreSame(departureAirport, flights[1].DepartureAirport);
            Assert.AreSame(arrivalAirport, flights[1].ArrivalAirport);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FlightsExistOnDate_ReturnsFlights()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            DateOnly date = new DateOnly(2025, 5, 5);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(date, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(1, flights.Count);
            Assert.AreEqual(date, flights[0].DepartureDate);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FlightsDoNotExistOnDate_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            DateOnly date = new DateOnly(2025, 5, 4);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(date, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_NoDirectFlightsExistBetweenAirports_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_NoFlightsExistBetweenAirports_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565051", LocalizedName = "London Gatwick", SkyId = "LGW" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FetchedOriginAirportEntityIdDoesNotMatchSpecifiedDepartureAirportEntityId_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565068", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FetchedOriginAirportSkyCodeDoesNotMatchSpecifiedDepartureAirportSkyId_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFL" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FetchedDestinationAirportEntityIdDoesNotMatchSpecifiedArrivalAirportEntityId_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565060", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FetchedDestinationAirportSkyCodeDoesNotMatchSpecifiedArrivalAirportSkyId_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHS" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }
        
        [TestMethod]
        public async Task GetFlightsAsync_DataNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""missing"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_FlightQuotesNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""results"":
                        [{
                            ""content"": {
                                ""direct"": false,
                                ""outboundLeg"": {
                                    ""destinationAirport"": {
                                        ""id"": ""95565050"",
                                        ""name"": ""LHR"",
                                        ""skyCode"": ""LHR"",
                                        ""type"": ""Airport""
                                    },
                                    ""localDepartureDate"": ""2025-05-12"",
                                    ""localDepartureDateLabel"": ""Mon, May 12"",
                                    ""originAirport"": {
                                        ""id"": ""95565058"",
                                        ""name"": ""JFK"",
                                        ""skyCode"": ""JFK"",
                                        ""type"": ""Airport""
                                    }
                                }
                            },
                            ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                        },
                        {
                            ""content"": {
                                ""direct"": true,
                                ""outboundLeg"": {
                                    ""destinationAirport"": {
                                        ""id"": ""95565050"",
                                        ""name"": ""LHR"",
                                        ""skyCode"": ""LHR"",
                                        ""type"": ""Airport""
                                    },
                                    ""localDepartureDate"": ""2025-05-02"",
                                    ""localDepartureDateLabel"": ""Fri, May 2"",
                                    ""originAirport"": {
                                        ""id"": ""95565058"",
                                        ""name"": ""JFK"",
                                        ""skyCode"": ""JFK"",
                                        ""type"": ""Airport""
                                    }
                                }
                            },
                            ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                        },
                        {
                            ""content"": {
                                ""direct"": true,
                                ""outboundLeg"": {
                                    ""destinationAirport"": {
                                        ""id"": ""95565050"",
                                        ""name"": ""LHR"",
                                        ""skyCode"": ""LHR"",
                                        ""type"": ""Airport""
                                    },
                                    ""localDepartureDate"": ""2025-05-05"",
                                    ""localDepartureDateLabel"": ""Mon, May 5"",
                                    ""originAirport"": {
                                        ""id"": ""95565058"",
                                        ""name"": ""JFK"",
                                        ""skyCode"": ""JFK"",
                                        ""type"": ""Airport""
                                    }
                                }
                            },
                            ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                        }
                    ]
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ResultsNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""missing"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_ContentNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""missing"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""missing"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""missing"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_DirectNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_OutboundNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""missing"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""missing"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""missing"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_OriginAirportNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""depatureAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""depatureAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""depatureAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_DestinationAirportNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""arrivalAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040709*I*JFK*LHR*20250512*smtf*FI""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""arrivalAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040246*D*JFK*LHR*20250502*airf*AF""
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""arrivalAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                },
                                ""id"": ""{bl}:202504040539*D*JFK*LHR*20250505*airf*AF""
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task GetFlightsAsync_IdNodeMissing_ReturnsEmptyList()
        {
            // Arrange
            string json = @"
                {
                    ""data"":
                    {
                        ""flightQuotes"":
                        {
                            ""results"":
                            [{
                                ""content"": {
                                    ""direct"": false,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-12"",
                                        ""localDepartureDateLabel"": ""Mon, May 12"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                }
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-02"",
                                        ""localDepartureDateLabel"": ""Fri, May 2"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                }
                            },
                            {
                                ""content"": {
                                    ""direct"": true,
                                    ""outboundLeg"": {
                                        ""destinationAirport"": {
                                            ""id"": ""95565050"",
                                            ""name"": ""LHR"",
                                            ""skyCode"": ""LHR"",
                                            ""type"": ""Airport""
                                        },
                                        ""localDepartureDate"": ""2025-05-05"",
                                        ""localDepartureDateLabel"": ""Mon, May 5"",
                                        ""originAirport"": {
                                            ""id"": ""95565058"",
                                            ""name"": ""JFK"",
                                            ""skyCode"": ""JFK"",
                                            ""type"": ""Airport""
                                        }
                                    }
                                }
                            }
                        ]
                    }
                }
            }";
            Setup(json);
            AirportDto departureAirport = new AirportDto { EntityId = "95565058", LocalizedName = "New York John F. Kennedy", SkyId = "JFK" };
            AirportDto arrivalAirport = new AirportDto { EntityId = "95565050", LocalizedName = "London Heathrow", SkyId = "LHR" };

            // Act
            var flights = await _flightService.GetFlightsAsync(null, departureAirport, arrivalAirport);

            // Assert
            Assert.IsNotNull(flights);
            Assert.AreEqual(0, flights.Count);
        }

        // -------------------- Helper Methods --------------------
        private void Setup(string json)
        {
            _mockHttpHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

            _flightService = new FlightService(_httpClient, _mockConfig.Object);
        }
    }
}