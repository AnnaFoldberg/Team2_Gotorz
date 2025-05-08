using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Controllers;
using Gotorz.Server.Services;
using Gotorz.Shared.DTOs;
using Gotorz.Server.Models;

namespace Gotorz.Server.UnitTests.Controllers
{
    [TestClass]
    public class HotelBookingControllerTests
    {
        private Mock<IHotelBookingService> _mockService = null!;
        private HotelBookingController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IHotelBookingService>();
            _controller = new HotelBookingController(_mockService.Object);
        }

        [TestMethod]
        public async Task AddHotelBooking_ValidBookingDto_ReturnsOk()
        {
            // Arrange
            var bookingDto = new HotelBookingDto
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                HotelId = 1,
                HotelRoomId = 1,
                Price = 100,
                RoomCapacity = 2,
                HolidayPackageId = 10
            };

            _mockService.Setup(s => s.AddHotelBookingAsync(It.IsAny<HotelBooking>()))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            _mockService.Verify(s => s.AddHotelBookingAsync(It.IsAny<HotelBooking>()), Times.Once);
        }
    }
}