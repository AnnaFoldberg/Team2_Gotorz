using Bunit;
using Bunit.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Moq;
using System.Net;
using Gotorz.Client.Services;
using Gotorz.Shared.DTOs;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the <see cref="Login"/> component.
    /// </summary>
    /// <author>Eske</author>
    [TestClass]
    public class LoginTests : Bunit.TestContext
    {
        [TestInitialize]
        public void Setup()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetNotAuthorized(); // simulate a logged-out user

            if (!Services.Any(s => s.ServiceType == typeof(IUserService)))
            {
                var defaultUserService = new Mock<IUserService>();
                defaultUserService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                    .ReturnsAsync((true, null));

                Services.AddSingleton(defaultUserService.Object);
            }
        }

        [TestMethod]
        public void Submit_MissingFields_ShowsValidationMessages()
        {
            // Arrange
            var component = RenderComponent<Login>();

            // Act
            component.Find("form").Submit();

            // Assert
            var markup = component.Markup;
            Assert.IsTrue(markup.Contains("Email is required"));
            Assert.IsTrue(markup.Contains("Password is required"));
        }

        [TestMethod]
        public void Submit_InvalidEmail_ShowsValidationMessage()
        {
            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("notanemail");
            component.Find("input[id=password]").Change("ValidPass1");
            component.Find("form").Submit();

            Assert.IsTrue(component.Markup.Contains("Invalid email"));
        }

        [TestMethod]
        public void Submit_EmptyPassword_ShowsRequiredMessage()
        {
            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("test@example.com");
            component.Find("input[id=password]").Change("");
            component.Find("form").Submit();

            Assert.IsTrue(component.Markup.Contains("Password is required"));
        }

        [TestMethod]
        public void Submit_ValidCredentials_NoValidationErrors()
        {
            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("test@example.com");
            component.Find("input[id=password]").Change("ValidPass1");
            component.Find("form").Submit();

            var errors = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void Submit_InvalidCredentials_ShowsErrorMessage()
        {
            // Arrange
            var errorText = "Forkert email eller adgangskode";

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync((false, errorText));

            Services.AddSingleton(mockUserService.Object);

            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("wrong@example.com");
            component.Find("input[id=password]").Change("WrongPass123");
            component.Find("form").Submit();

            // Act
            var markup = component.Markup;

            // Assert
            Assert.IsTrue(markup.Contains(errorText));
        }
    }
}
