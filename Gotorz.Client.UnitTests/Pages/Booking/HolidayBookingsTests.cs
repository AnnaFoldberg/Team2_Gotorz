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
    /// Contains unit tests for the <see cref="HolidayBookings"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class HolidayBookingsTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<HolidayBookings>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<HolidayBookings>>();

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
        public void OnInitializedAsync_IsAdmin_ShowsHolidayBookings()
        {
            // Arrange
            SetUser("admin");

            var mockCustomerOne = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerTwo = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookings = new List<HolidayBookingDto>()
            {
                new HolidayBookingDto
                {
                    BookingReference = "G01",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G02",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G03",
                    Customer = mockCustomerTwo,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(3, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
            Assert.IsTrue(bookingComponents.ElementAt(2).Instance.BookingReference == "G03");
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsHolidayBookingsWithEditButton()
        {
            // Arrange
            SetUser("sales");

            var mockCustomerOne = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerTwo = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookings = new List<HolidayBookingDto>()
            {
                new HolidayBookingDto
                {
                    BookingReference = "G01",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G02",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G03",
                    Customer = mockCustomerTwo,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(3, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
            Assert.IsTrue(bookingComponents.ElementAt(2).Instance.BookingReference == "G03");
            var buttons = component.FindAll("button");
            var editButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Edit"));
            Assert.IsNotNull(editButton);
            Assert.IsFalse(editButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsCustomer_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var mockCustomerOne = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerTwo = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookings = new List<HolidayBookingDto>()
            {
                new HolidayBookingDto
                {
                    BookingReference = "G01",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G02",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G03",
                    Customer = mockCustomerTwo,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_NotLoggedIn_ShowsLogin()
        {
            // Arrange
            // Arrange
            SetUser(null);

            var mockCustomerOne = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerTwo = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookings = new List<HolidayBookingDto>()
            {
                new HolidayBookingDto
                {
                    BookingReference = "G01",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G02",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G03",
                    Customer = mockCustomerTwo,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Holiday Booking"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_MultipleHolidayBookingsExist_ShowsHolidayBookings()
        {
            // Arrange
            SetUser("sales");

            var mockCustomerOne = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerTwo = new UserDto
            {
                UserId = "3a7b4ccf-2d09-4753-80ba-76818cd4f3f1"
            };

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookings = new List<HolidayBookingDto>()
            {
                new HolidayBookingDto
                {
                    BookingReference = "G01",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G02",
                    Customer = mockCustomerOne,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                },
                new HolidayBookingDto
                {
                    BookingReference = "G03",
                    Customer = mockCustomerTwo,
                    Status = BookingStatus.Pending,
                    HolidayPackage = mockHolidayPackage
                }
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(3, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
            Assert.IsTrue(bookingComponents.ElementAt(2).Instance.BookingReference == "G03");
            var buttons = component.FindAll("button");
            var editButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Edit"));
            Assert.IsNotNull(editButton);
            Assert.IsFalse(editButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_SingleHolidayBookingsExist_ShowsHolidayBookings()
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

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(new List<HolidayBookingDto>{ mockHolidayBooking });

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(1, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            var buttons = component.FindAll("button");
            var editButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Edit"));
            Assert.IsNotNull(editButton);
            Assert.IsFalse(editButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void OnInitializedAsync_NullHolidayBookings_ShowsPageNotFound()
        {
            // Arrange
            SetUser("sales");

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync((List<HolidayBookingDto>)null);

            // Act
            var component = RenderComponent<HolidayBookings>();

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

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync())
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<HolidayBookings>();

            // Assert
            _mockBookingService.Verify(s => s.GetAllHolidayBookingsAsync(), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error initializing holiday bookings")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // -------------------- NavigateToHolidayBooking --------------------
        [TestMethod]
        public async Task NavigateToHolidayBookingAsync_ValidBookingReference_NavigatesToHolidayBooking()
        {
            // Arrange
            SetUser("sales");

            var bookingReference = "G01";

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
                BookingReference = bookingReference,
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetAllHolidayBookingsAsync()).ReturnsAsync(new List<HolidayBookingDto>{ mockHolidayBooking });

            var component = RenderComponent<HolidayBookings>();
            var buttons = component.FindAll("button");
            var editButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Edit"));

            // Act
            editButton.Click();

            // Assert
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains($"/booking/holiday-booking/{bookingReference}/edit?From=all-holiday-bookings"));
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