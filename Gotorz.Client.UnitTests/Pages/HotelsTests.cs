using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Gotorz.Shared.Models;

namespace Gotorz.Client.UnitTests.Pages
{
    [TestClass]
    public class HotelsTests : Bunit.TestContext
    {
        private Mock<IHotelService> _mockHotelService;

        [TestInitialize]
        public void Setup()
        {
            _mockHotelService = new Mock<IHotelService>();
            Services.AddSingleton(_mockHotelService.Object);
        }

        /// <summary>
        /// Verifies that submitting the form without entering a city 
        /// triggers a validation error saying "City is required".
        /// </summary>
        [TestMethod]
        public void Form_MissingCity_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Hotels>();
            component.Find("#country").Change("Denmark"); // City left empty

            // Act
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("City is required"));
        }

        /// <summary>
        /// Verifies that submitting the form without entering a country 
        /// triggers a validation error saying "Country is required".
        /// </summary>
        [TestMethod]
        public void Form_MissingCountry_ShowsValidationMessage()
        {
            // Arrange
            var component = RenderComponent<Hotels>();
            component.Find("#city").Change("Copenhagen"); // Country left empty

            // Act
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("Country is required"));
        }
        /// <summary>
        /// Verifies that when the API returns an empty hotel list,
        /// the message "No hotels found." is shown to the user.
        /// </summary>
        [TestMethod]
        public async Task Search_WithNoHotels_ShowsNoHotelsFoundMessage()
        {
            // Arrange
            _mockHotelService.Setup(s => s.GetHotelsByCityName("Copenhagen", "Denmark", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Hotel>());

            var component = RenderComponent<Hotels>();

            // Fill required fields
            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");

            // Act
            await component.InvokeAsync(() =>
            {
                component.Find("form").Submit();
            });

            // Assert
            Assert.IsTrue(component.Markup.Contains("No hotels found."));
        }
    }
}