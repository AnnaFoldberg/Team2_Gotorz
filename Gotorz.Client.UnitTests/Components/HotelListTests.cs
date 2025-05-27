using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Gotorz.Shared.DTOs;
using Gotorz.Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gotorz.Client.UnitTests.Components
{
    [TestClass]
    public class HotelListTests : Bunit.TestContext
    {
        private Mock<IHotelService> _mockHotelService = null!;
        private Mock<IUserService> _mockUserService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockHotelService = new Mock<IHotelService>();
            _mockUserService = new Mock<IUserService>();

            Services.AddSingleton(_mockHotelService.Object);
            Services.AddSingleton(_mockUserService.Object);
            Services.AddSingleton(Mock.Of<IJSRuntime>());
        }

        [TestMethod]
        public void Form_MissingCity_ShowsValidationMessage()
        {
            // Arrange
            _mockUserService.Setup(x => x.IsUserInRoleAsync("sales")).ReturnsAsync(true);
            var component = RenderComponent<HotelList>();
            component.Find("#country").Change("Denmark");
            component.Find("form").Submit();

            // Assert
            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("City is required"));
        }

        [TestMethod]
        public void Form_MissingCountry_ShowsValidationMessage()
        {
            _mockUserService.Setup(x => x.IsUserInRoleAsync("sales")).ReturnsAsync(true);
            var component = RenderComponent<HotelList>();
            component.Find("#city").Change("Copenhagen");
            component.Find("form").Submit();

            var validationMessages = component.FindAll("div.validation-message, .validation-summary-errors");
            Assert.IsTrue(validationMessages.Count > 0);
            Assert.IsTrue(component.Markup.Contains("Country is required"));
        }

        [TestMethod]
        public async Task Search_WithNoHotels_ShowsNoHotelsFoundMessage()
        {
            _mockUserService.Setup(x => x.IsUserInRoleAsync("sales")).ReturnsAsync(true);
            _mockHotelService.Setup(s => s.GetHotelsByCityName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<HotelDto>());

            var component = RenderComponent<HotelList>();
            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");
            component.Find("form").Submit();
            await component.InvokeAsync(() => { });

            Assert.IsTrue(component.Markup.Contains("No hotels found."));
        }

        [TestMethod]
        public async Task SubmitBooking_InvokesEventCallback()
        {
            // Arrange
            var hotel = new HotelDto { Name = "Hotel Test", ExternalHotelId = "EXT123" };
            var room = new HotelRoomDto
            {
                Name = "Test Room",
                ExternalRoomId = "ROOM123",
                Capacity = 2,
                Price = 100,
                MealPlan = "Dinner",
                Refundable = true
            };

            _mockUserService.Setup(x => x.IsUserInRoleAsync("sales")).ReturnsAsync(true);
            _mockHotelService.Setup(s => s.GetHotelsByCityName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<HotelDto> { hotel });

            _mockHotelService.Setup(s => s.GetHotelRoomsByHotelId(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<HotelRoomDto> { room });

            HotelBookingDto? receivedBooking = null;

            var component = RenderComponent<HotelList>(parameters =>
            {
                parameters.Add(p => p.OnHotelBookingConfirmed, EventCallback.Factory.Create<HotelBookingDto>(this, b => receivedBooking = b));
            });

            component.Find("#country").Change("Denmark");
            component.Find("#city").Change("Copenhagen");
            component.Find("form").Submit();
            await component.InvokeAsync(() => { });

            component.Find("button.btn-outline-secondary").Click();
            await component.InvokeAsync(() => { });

            component.Find("button.btn-success").Click();
            await component.InvokeAsync(() => { });

            Assert.IsNotNull(receivedBooking);
            Assert.AreEqual("ROOM123", receivedBooking.HotelRoom.ExternalRoomId);
        }
    }
}