using Bunit;
using Bunit.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Moq;
using System.Net;

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
            authContext.SetAuthorized("Test user");
            authContext.SetClaims(new Claim(ClaimTypes.Name, "Test user"));

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"result\":true}")
                });

            var client = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://localhost")
            };

            Services.AddSingleton<HttpClient>(client);
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
            Assert.IsTrue(markup.Contains("Email er påkrævet"));
            Assert.IsTrue(markup.Contains("Adgangskode er påkrævet"));
        }

        [TestMethod]
        public void Submit_InvalidEmail_ShowsValidationMessage()
        {
            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("notanemail");
            component.Find("input[id=password]").Change("ValidPass1");
            component.Find("form").Submit();

            Assert.IsTrue(component.Markup.Contains("Ugyldig email"));
        }

        [TestMethod]
        public void Submit_EmptyPassword_ShowsRequiredMessage()
        {
            var component = RenderComponent<Login>();
            component.Find("input[id=email]").Change("test@example.com");
            component.Find("input[id=password]").Change(""); // empty triggers [Required]
            component.Find("form").Submit();

            Assert.IsTrue(component.Markup.Contains("Adgangskode er påkrævet"));
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

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent(errorText)
                });

            var client = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://localhost")
            };
            Services.AddSingleton<HttpClient>(client);

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
