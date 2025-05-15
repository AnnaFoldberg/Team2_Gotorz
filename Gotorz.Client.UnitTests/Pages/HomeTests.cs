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
using Microsoft.AspNetCore.Components;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="Home"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class HomeTests : Bunit.TestContext
    {
        // private Mock<HolidayPackageService> _mockHolidayPackageService;
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<Home>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            // _mockHolidayPackageService = new Mock<HolidayPackageService>();
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<Home>>();

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
            // Services.AddSingleton(_mockHolidayPackageService.Object);
            Services.AddSingleton(_mockBookingService.Object);
            Services.AddSingleton(_mockUserService.Object);
            Services.AddSingleton(logger.Object);
        }

        // -------------------- OnInitializedAsync --------------------
        // [TestMethod]
        // public void OnInitializedAsync_NotLoggedIn_ShowsHolidayPackageList()
        // {
        //     // Arrange
        //     SetUser(null);

        //     // Act
        //     var component = RenderComponent<Home>();

        //     // Assert
        //     var holidayPackageListComponent = component.FindComponents<HolidayPackageList>();
        //     Assert.AreEqual(1, holidayPackageListComponent.Count);
        // }

        [TestMethod]
        public void OnInitializedAsync_IsCustomer_ShowsHolidayPackageListAndCustomerHolidayBookings()
        {
            // Arrange
            SetUser("customer");

            // Act
            var component = RenderComponent<Home>();

            // Assert
            // var holidayPackageListComponent = component.FindComponents<HolidayPackageList>();
            // Assert.AreEqual(1, holidayPackageListComponent.Count);
            var customerHolidayBookingsComponent = component.FindComponents<CustomerHolidayBookings>();
            Assert.AreEqual(1, customerHolidayBookingsComponent.Count);
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsHolidayBookings()
        {
            // Arrange
            SetUser("sales");

            // Act
            var component = RenderComponent<Home>();

            // Assert
            var holidayBookingsComponent = component.FindComponents<HolidayBookings>();
            Assert.AreEqual(1, holidayBookingsComponent.Count);
        }

        [TestMethod]
        public void OnInitializedAsync_IsAdmin_ShowsUserListAndHolidayBookings()
        {
            // Arrange
            SetUser("admin");

            // Act
            var component = RenderComponent<Home>();

            // Assert
            var userListComponent = component.FindComponents<UserList>();
            Assert.AreEqual(1, userListComponent.Count);
            var holidayBookingsComponent = component.FindComponents<HolidayBookings>();
            Assert.AreEqual(1, holidayBookingsComponent.Count);
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
            _mockUserService.Setup(s => s.GetUserRoleAsync()).ReturnsAsync(role);
        }
    }
}