using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
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
    /// Contains unit tests for the <see cref="RequestConfirmation"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class RequestConfirmationTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<RequestConfirmation>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<RequestConfirmation>>();

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
        public void OnInitializedAsync_IsAuthorizedCustomer_ShowsRequestConfirmation()
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

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains($"Destination: Rome"));
            Assert.IsTrue(component.Markup.Contains($"IBAN XX7050518371227343"));
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

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("unauthorizedcustomer@mail.com");

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsPageNotFound()
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

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_IsAdmin_ShowsPageNotFound()
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

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
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
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Booking Request Confirmation"));
            Assert.IsFalse(component.Markup.Contains("Holiday Package"));
            Assert.IsFalse(component.Markup.Contains("Travellers"));
        }

        // -------------------- OnInitializedAsync --------------------
                [TestMethod]
        public void OnInitializedAsync_HolidayBookingExists_ShowsRequestConfirmation()
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
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockBookingService.Setup(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockTravellers);

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains($"Destination: Rome"));
            Assert.IsTrue(component.Markup.Contains("Travellers"));
            Assert.IsTrue(component.Markup.Contains($"IBAN XX7050518371227343"));
            _mockUserService.Verify(s => s.GetEmailAsync(), Times.Exactly(2));
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference), Times.Once());
            _mockBookingService.Verify(s => s.GetTravellersAsync(mockHolidayBooking.BookingReference), Times.Once());
        }

        [TestMethod]
        public void OnInitializedAsync_NullHolidayBooking_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var bookingReference = "G01";

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(bookingReference)).ReturnsAsync((HolidayBookingDto)null);

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(p => p.BookingReference, null));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
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
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<RequestConfirmation>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error initializing request confirmation")),
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