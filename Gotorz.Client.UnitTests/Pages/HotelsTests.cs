using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Gotorz.Shared.DTOs;
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

        [TestMethod]
        public void Form_MissingCity_ShowsValidationMessage()
        {
            var component = RenderComponent<Hotels>();
            component.Find("#country").Change("Denmark");
            component.Find("form").Submit();

            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("City is required"));
        }

        [TestMethod]
        public void Form_MissingCountry_ShowsValidationMessage()
        {
            var component = RenderComponent<Hotels>();
            component.Find("#city").Change("Copenhagen");
            component.Find("form").Submit();

            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("Country is required"));
        }

        [TestMethod]
        public async Task Search_WithNoHotels_ShowsNoHotelsFoundMessage()
        {
            _mockHotelService.Setup(s => s.GetHotelsByCityName("Copenhagen", "Denmark", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<HotelDto>());

            var component = RenderComponent<Hotels>();
            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");

            await component.InvokeAsync(() => component.Find("form").Submit());

            Assert.IsTrue(component.Markup.Contains("No hotels found."));
        }

        [TestMethod]
        public async Task SubmitBooking_Success_ShowsSuccessMessage()
        {
            var hotelId = 1;
            var roomId = 101;

            var mockRooms = new List<HotelRoomDto>
            {
                new HotelRoomDto
                {
                    HotelRoomId = roomId,
                    Name = "Test Room",
                    Capacity = 2,
                    Price = 100,
                    MealPlan = "Breakfast",
                    Refundable = true
                }
            };

            var mockHotels = new List<HotelDto>
            {
                new HotelDto
                {
                    HotelId = hotelId,
                    Name = "Test Hotel",
                    Address = "Test Address",
                    Rating = 4,
                    ExternalHotelId = "EXT123"
                }
            };

            _mockHotelService.Setup(s => s.GetHotelsByCityName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(mockHotels);

            _mockHotelService.Setup(s => s.GetHotelRoomsByHotelId(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(mockRooms);

            _mockHotelService.Setup(s => s.BookHotelAsync(It.IsAny<HotelBookingDto>()))
                .Returns(Task.CompletedTask);

            var component = RenderComponent<Hotels>();
            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");
            component.Find("#arrivalDate").Change(DateTime.Today.ToString("yyyy-MM-dd"));
            component.Find("#departureDate").Change(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));

            component.Find("form").Submit();
            await component.InvokeAsync(() => { });

            component.Find("button.btn-outline-secondary").Click();
            await component.InvokeAsync(() => { });

            component.Find("button.btn-success").Click();

            var successMessage = component.Find("div.alert.alert-success");
            Assert.IsNotNull(successMessage);
            Assert.IsTrue(successMessage.InnerHtml.Contains("Booking was successful"));
        }
    }
}
