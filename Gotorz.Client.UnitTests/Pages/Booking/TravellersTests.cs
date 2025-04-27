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

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="Travellers"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class TravellersTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<ILogger<RequestConfirmation>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
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
            Services.AddSingleton(logger.Object);
        }

        // -------------------- OnInitializedAsync --------------------

        // -------------------- AddTraveller --------------------

        // -------------------- RemoveTraveller --------------------

        // -------------------- ConfirmAndPostTravellersAsync --------------------
    }
}