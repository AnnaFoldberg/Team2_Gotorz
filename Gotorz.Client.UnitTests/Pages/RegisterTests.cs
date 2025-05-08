using Bunit;
using Bunit.TestDoubles;
using Gotorz.Client.Pages;
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
            authContext.SetClaims(
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.Role, "admin")
            );
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
            Assert.IsTrue(component.Markup.Contains("Email er påkrævet"));
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
            Assert.IsTrue(component.Markup.Contains("Ugyldig email"));
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
            Assert.IsTrue(markup.Contains("Email er påkrævet"));
            Assert.IsTrue(markup.Contains("Fornavn er påkrævet"));
            Assert.IsTrue(markup.Contains("Efternavn er påkrævet"));
            Assert.IsTrue(markup.Contains("Telefonnummer er påkrævet"));
            Assert.IsTrue(markup.Contains("Adgangskode er påkrævet"));
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
            Assert.IsTrue(markup.Contains("Adgangskoden skal være mindst 6 tegn."));
            Assert.IsTrue(markup.Contains("Adgangskoden skal indeholde både store og små bogstaver."));
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
