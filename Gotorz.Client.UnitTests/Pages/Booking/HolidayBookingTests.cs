using Bunit;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Gotorz.Shared.DTOs;
using Bunit.TestDoubles;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Gotorz.Client.Pages.Booking;
using Gotorz.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="HolidayBooking"/> page.
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
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();
            Services.AddSingleton(logger.Object);
        }

        // -------------------- AuthorizeView --------------------
        [TestMethod]
        public void OnInitializedAsync_IsAuthorizedCustomer_ShowsHolidayBookingWithStatusInputDisabled()
        {
            // Arrange
            SetUser("customer");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomer);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains("Rome"));
            var status = component.Find("#status");
            Assert.IsTrue(status.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsSalesAndEditMode_ShowsHolidayBookingWithStatusInputEnabled()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters
                .Add(c => c.BookingReference, mockHolidayBooking.BookingReference)
                .Add(c => c.Mode, "edit"));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains("Rome"));
            var status = component.Find("#status");
            Assert.IsFalse(status.HasAttribute("disabled"));
            var confirmButton = component.Find("button[type='submit']");
            Assert.IsNotNull(confirmButton);
        }

        [TestMethod]
        public void OnInitializedAsync_IsSalesAndModeIsNull_ShowsHolidayBookingWithStatusInputDisabled()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters
                .Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains("Rome"));
            var status = component.Find("#status");
            Assert.IsTrue(status.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsAdmin_ShowsHolidayBookingWithStatusInputDisabled()
        {
            // Arrange
            SetUser("admin");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains("Rome"));
            var selectStatus = component.Find("#status");
            Assert.IsTrue(selectStatus.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsUnauthorizedCustomer_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockUnauthorizedCustomer = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockUnauthorizedCustomer);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_NotLoggedIn_ShowsLogin()
        {
            // Arrange
            SetUser(null);

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Holiday Booking"));
            Assert.IsFalse(component.Markup.Contains("Holiday Package"));
            Assert.IsFalse(component.Markup.Contains("Travellers"));
            Assert.IsFalse(component.Markup.Contains("Status"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_HolidayBookingExists_ShowsHolidayBooking()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters
                .Add(c => c.BookingReference, mockHolidayBooking.BookingReference)
                .Add(c => c.Mode, "edit"));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Booking Reference: #{mockHolidayBooking.BookingReference}"));
            Assert.IsTrue(component.Markup.Contains("Rome"));
            var status = component.Find("#status");
            Assert.IsFalse(status.HasAttribute("disabled"));
            var confirmButton = component.Find("button[type='submit']");
            Assert.IsNotNull(confirmButton);
        }

        [TestMethod]
        public void OnInitializedAsync_NullHolidayBooking_ShowsPageNotFound()
        {
            // Arrange
            SetUser("sales");

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(It.IsAny<string>())).ReturnsAsync((HolidayBookingDto)null);

            // Act
            var component = RenderComponent<HolidayBooking>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_ThrowsException_LogsError()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(It.IsAny<string>()), Times.Once());
            
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

        // -------------------- ConfirmChangesAsync --------------------
        [TestMethod]
        public async Task ConfirmChangesAsync_ValidBooking_CallsPatchHolidayBookingStatusAsync()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.PatchHolidayBookingStatusAsync(mockHolidayBooking), Times.Once());
        }

        [TestMethod]
        public async Task ConfirmChangesAsync_FromParameterIsAllHolidayBookings_NavigatesToFrom()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<HolidayBooking>(parameters => parameters
                .Add(c => c.BookingReference, mockHolidayBooking.BookingReference)
                .Add(c => c.From, "all-holiday-bookings"));

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains("/booking/all-holiday-bookings"));
        }

        [TestMethod]
        public async Task ConfirmChangesAsync_FromParameterIsCustomerHolidayBookings_NavigatesToFrom()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<HolidayBooking>(parameters => parameters
                .Add(c => c.BookingReference, mockHolidayBooking.BookingReference)
                .Add(c => c.From, $"customer/holiday-bookings?UserId={mockCustomer.UserId}"));

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains($"customer/holiday-bookings?UserId={mockCustomer.UserId}"));
        }

        [TestMethod]
        public async Task ConfirmChangesAsync_ThrowsException_LogsError()
        {
            // Arrange
            SetUser("sales");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockBookingService.Setup(s => s.PatchHolidayBookingStatusAsync(mockHolidayBooking))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            var component = RenderComponent<HolidayBooking>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference), Times.Once());
            _mockBookingService.Verify(s => s.PatchHolidayBookingStatusAsync(mockHolidayBooking), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error patching holiday booking")),
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