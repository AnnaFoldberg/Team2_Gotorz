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

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="CustomerHolidayBookings"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class CustomerHolidayBookingsTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<CustomerHolidayBookings>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<CustomerHolidayBookings>>();

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
        public void OnInitializedAsync_IsAdmin_ShowsCustomerHolidayBookings()
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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            var mockCustomerTwoHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G03",
                Customer = mockCustomerTwo,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };


            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerOne);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomerOne.UserId)).ReturnsAsync(mockCustomerOneHolidayBookings);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>(parameters => parameters.Add(c => c.UserId, mockCustomerOne.UserId));

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(2, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
        }

        [TestMethod]
        public void OnInitializedAsync_IsCustomer_ShowsCustomerHolidayBookings()
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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            var mockCustomerTwoHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G03",
                Customer = mockCustomerTwo,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };


            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerOne);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomerOne.UserId)).ReturnsAsync(mockCustomerOneHolidayBookings);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(2, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
        }

        [TestMethod]
        public void OnInitializedAsync_IsUnauthorizedCustomer_ShowsPageNotFound()
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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerTwo);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>(parameters => parameters.Add(c => c.UserId, mockCustomerOne.UserId));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsPageNotFound()
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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            var mockCustomerTwoHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G03",
                Customer = mockCustomerTwo,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerOne);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomerOne.UserId)).ReturnsAsync(mockCustomerOneHolidayBookings);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>(parameters => parameters.Add(c => c.UserId, mockCustomerOne.UserId));

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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            var mockCustomerTwoHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G03",
                Customer = mockCustomerTwo,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };


            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerOne);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomerOne.UserId)).ReturnsAsync(mockCustomerOneHolidayBookings);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Holiday Booking"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_MultipleCustomerHolidayBookingsExist_ShowsCustomerHolidayBookings()
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

            var mockCustomerOneHolidayBookings = new List<HolidayBookingDto>()
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
                }
            };

            var mockCustomerTwoHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G03",
                Customer = mockCustomerTwo,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };


            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomerOne);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomerOne.UserId)).ReturnsAsync(mockCustomerOneHolidayBookings);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(2, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
            Assert.IsTrue(bookingComponents.ElementAt(1).Instance.BookingReference == "G02");
        }

        [TestMethod]
        public void OnInitializedAsync_SingleCustomerHolidayBookingExists_ShowsCustomerHolidayBooking()
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

            var mockCustomerHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomer);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomer.UserId)).ReturnsAsync(new List<HolidayBookingDto>{mockCustomerHolidayBooking});

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            var bookingComponents = component.FindComponents<HolidayBooking>();
            Assert.AreEqual(1, bookingComponents.Count);
            Assert.IsTrue(bookingComponents.ElementAt(0).Instance.BookingReference == "G01");
        }

        [TestMethod]
        public void OnInitializedAsync_NullCustomerHolidayBookings_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var mockCustomer = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomer);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomer.UserId)).ReturnsAsync((List<HolidayBookingDto>)null);

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_NullUserId_ShowsPageNotFound()
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

            var mockCustomerHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                Customer = mockCustomer,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomer);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomer.UserId)).ReturnsAsync(new List<HolidayBookingDto>{mockCustomerHolidayBooking});

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_ThrowsException_LogsError()
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

            _mockUserService.Setup(s => s.GetCurrentUserAsync()).ReturnsAsync(mockCustomer);
            _mockBookingService.Setup(s => s.GetCustomerHolidayBookingsAsync(mockCustomer.UserId))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<CustomerHolidayBookings>();

            // Assert
            _mockBookingService.Verify(s => s.GetCustomerHolidayBookingsAsync(mockCustomer.UserId), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error initializing customer's holiday bookings")),
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