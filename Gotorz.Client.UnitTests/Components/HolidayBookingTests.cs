using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Gotorz.Client.Components;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Gotorz.Shared.DTOs;
using Bunit.TestDoubles;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Gotorz.Client.Pages.Booking;
using Gotorz.Shared.Enums;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="HolidayBooking"/> component.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class HolidayBookingTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<HolidayBooking>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<HolidayBooking>>();

            Services.AddOptions();

            Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>();
                return new DefaultAuthorizationPolicyProvider(options);
            });

            var mockAuthService = new Mock<IAuthorizationService>();
            mockAuthService
                .Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(_mockBookingService.Object);
            Services.AddSingleton(_mockUserService.Object);
            Services.AddSingleton(logger.Object);
        }

        // -------------------- AuthorizeView --------------------
        [TestMethod]
        public void OnInitializedAsync_IsAdmin_ShowsHolidayBooking()
        {
            // Arrange            
            SetUser("admin");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            var mockTravellers = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBooking}
            };

            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockTravellers);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Destination: Rome"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsHolidayBooking()
        {
            // Arrange            
            SetUser("sales");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            var mockTravellers = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBooking}
            };

            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockTravellers);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Destination: Rome"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsAuthorizedCustomer_ShowsHolidayBooking()
        {
            // Arrange            
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            var mockTravellers = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBooking}
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockTravellers);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Destination: Rome"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsUnauthorizedCustomer_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("unauthorizedcustomer@mail.com");
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync((IEnumerable<TravellerDto>)null);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
            _mockUserService.Verify(s => s.GetEmailAsync(), Times.Once);
            _mockBookingService.Verify(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference), Times.Never);
        }

        [TestMethod]
        public void OnInitializedAsync_NotLoggedIn_ShowsLogin()
        {
            // Arrange            
            SetUser(null);

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Holiday Package"));
            Assert.IsFalse(component.Markup.Contains("Travellers"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_HolidayBookingAndTravellersExist_ShowsHolidayBookingAndTravellers()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            var mockTravellers = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBooking}
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockTravellers);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Destination: Rome"));
            Assert.IsTrue(component.Markup.Contains("Traveller 1. Age: 21"));
            Assert.IsTrue(component.Markup.Contains("Passport number: P1"));
            Assert.IsTrue(component.Markup.Contains("Traveller 2. Age: 22"));
            Assert.IsTrue(component.Markup.Contains("Passport number: P2"));
            Assert.IsTrue(component.Markup.Contains("Traveller 3. Age: 23"));
            Assert.IsTrue(component.Markup.Contains("Passport number: P3"));
            _mockUserService.Verify(s => s.GetEmailAsync(), Times.Once);
            _mockBookingService.Verify(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference), Times.Once());
        }

        [TestMethod]
        public void OnInitializedAsync_NullHolidayBooking_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, null));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_NullTravellers_ShowsLoading()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync((IEnumerable<TravellerDto>)null);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Loading ..."));
            _mockUserService.Verify(s => s.GetEmailAsync(), Times.Once);
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync((IEnumerable<TravellerDto>)null);
        }

        [TestMethod]
        public async Task OnInitializedAsync_ThrowsException_LogsError()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Destination = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            var mockTravellers = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBooking},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBooking}
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(p => p.HolidayBookingDto, mockHolidayBooking));

            // Assert
            _mockBookingService.Verify(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error initializing holiday booking")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // -------------------- Helper Methods --------------------
        private void SetUser(string? role)
        {
            var authContext = this.AddTestAuthorization();
            if (role == null)
            {
                authContext.SetNotAuthorized();
                return;
            }

            authContext.SetAuthorized("Test user");
            authContext.SetClaims(
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.Role, role)
            );
            _mockUserService.Setup(s => s.IsUserInRoleAsync(role)).ReturnsAsync(true);
        }
    }
}