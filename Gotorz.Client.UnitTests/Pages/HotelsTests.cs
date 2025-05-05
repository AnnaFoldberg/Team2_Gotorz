using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Gotorz.Shared.Models;
using Gotorz.Client.Pages;

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
        /// <summary>
        /// Verifies that booking a hotel triggers success message.
        /// </summary>
        [TestMethod]
        public async Task SubmitBooking_Success_ShowsSuccessMessage()
        {
            // Arrange
            var hotelId = 1;
            var roomId = 101;

            var mockRooms = new List<HotelRoom>
    {
        new HotelRoom
        {
            HotelRoomId = roomId,
            Name = "Test Room",
            Capacity = 2,
            Price = 100,
            MealPlan = "Breakfast",
            Refundable = true
        }
    };

            var mockHotels = new List<Hotel>
    {
        new Hotel
        {
            HotelId = hotelId,
            Name = "Test Hotel",
            Address = "Test Address",
            Rating = 4,
            ExternalHotelId = "EXT123",
        }
    };

            _mockHotelService.Setup(s => s.GetHotelsByCityName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(mockHotels);

            _mockHotelService.Setup(s => s.GetHotelRoomsByHotelId(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(mockRooms);

            _mockHotelService.Setup(s => s.BookHotelAsync(It.IsAny<HotelBooking>()))
                .Returns(Task.CompletedTask);

            // Act
            var component = RenderComponent<Hotels>();

            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");
            component.Find("#arrivalDate").Change(DateTime.Today.ToString("yyyy-MM-dd"));
            component.Find("#departureDate").Change(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));

            component.Find("form").Submit();

            // Ensure component is fully rendered after async methods
            await component.InvokeAsync(() => { });

            // Expand details
            component.Find("button.btn-outline-secondary").Click();

            await component.InvokeAsync(() => { });

            // Click Book
            component.Find("button.btn-success").Click();

            // Assert
            var successMessage = component.Find("div.alert.alert-success");
            Assert.IsNotNull(successMessage);
            Assert.IsTrue(successMessage.InnerHtml.Contains("Booking was successful"));
        }
    }
}