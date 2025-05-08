using Bunit;
using Bunit.TestDoubles;
using Gotorz.Client.Pages;
using Gotorz.Client.Services;
using Gotorz.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;


namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the <see cref="Register"/> component.
    /// </summary>
    /// <author>Eske</author>
    [TestClass]
    public class RegisterTests : Bunit.TestContext
    {
        [TestInitialize]
        public void Setup()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("Test user");
            authContext.SetClaims(new Claim(ClaimTypes.Name, "Test user"), new Claim(ClaimTypes.Role, "admin"));

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                .ReturnsAsync((true, null));

            Services.AddSingleton(mockUserService.Object);
        }


        [TestMethod]
        public void Submit_MissingEmail_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Register>();

            // Act
            component.Find("form").Submit();

            // Assert
            var messages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(messages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("Email is required"));
        }

        [TestMethod]
        public void Submit_InvalidEmail_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Register>();

            component.Find("input[id=email]").Change("not-an-email");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("Invalid email"));
        }

        [TestMethod]
        public void Submit_MissingRequiredFields_ShowsAllValidationMessages()
        {
            // Arrange
            var component = RenderComponent<Register>();

            // Act
            component.Find("form").Submit();

            // Assert
            var markup = component.Markup;
            Assert.IsTrue(markup.Contains("Email is required"));
            Assert.IsTrue(markup.Contains("First name is required"));
            Assert.IsTrue(markup.Contains("Last name is required"));
            Assert.IsTrue(markup.Contains("Phonenumber is required"));
            Assert.IsTrue(markup.Contains("Password is required"));
        }

        [TestMethod]
        public void Submit_InvalidPasswordFormat_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Register>();

            component.Find("input[id=email]").Change("test@example.com");
            component.Find("input[id=firstName]").Change("Anna");
            component.Find("input[id=lastName]").Change("Smith");
            component.Find("input[id=phone]").Change("12345678");
            component.Find("input[id=password]").Change("abc"); // too short and no uppercase

            // Act
            component.Find("form").Submit();

            // Assert
            var markup = component.Markup;
            Assert.IsTrue(markup.Contains("Password must be at least 6 characters long"));
            Assert.IsTrue(markup.Contains("Password must include both uppercase and lowercase letters"));
        }

        [TestMethod]
        public void Submit_ValidInput_NoValidationErrors()
        {
            // Arrange
            var component = RenderComponent<Register>();

            component.Find("input[id=email]").Change("test@example.com");
            component.Find("input[id=firstName]").Change("Anna");
            component.Find("input[id=lastName]").Change("Smith");
            component.Find("input[id=phone]").Change("12345678");
            component.Find("input[id=password]").Change("Abcdef1");

            // Act
            component.Find("form").Submit();

            // Assert
            var validationErrors = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.AreEqual(0, validationErrors.Count);
        }
    }
}
