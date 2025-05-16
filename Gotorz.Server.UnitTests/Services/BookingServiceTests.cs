using Moq;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Server.Services;

namespace Gotorz.Server.UnitTests.Services
{
    /// <summary>
    /// Contains unit tests for the <see cref="BookingService"/> class.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class BookingServiceTests
    {
        private BookingService _bookingService;
        private Mock<IHolidayBookingRepository> _mockHolidayBookingRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHolidayBookingRepository = new Mock<IHolidayBookingRepository>();

            _bookingService = new BookingService(_mockHolidayBookingRepository.Object);
        }

        // -------------------- GenerateNextBookingReferenceAsync --------------------
        [TestMethod]
        public async Task GenerateNextBookingReference_HolidayBookingsExistInDatabase_ReturnsNextBookingReference()
        {
            // Arrange
            var expectedBookingReference = "G0004";

            var mockHolidayBookings = new List<HolidayBooking>
            {
                new HolidayBooking
                {
                    HolidayBookingId = 1,
                    BookingReference = "G0001"
                },
                new HolidayBooking
                {
                    HolidayBookingId = 2,
                    BookingReference = "G0002"
                },
                new HolidayBooking
                {
                    HolidayBookingId = 3,
                    BookingReference = "G0003"
                }
            };
            
            _mockHolidayBookingRepository.Setup(s => s.GetAllAsync()).ReturnsAsync(mockHolidayBookings);

            // Act
            var bookingReference = await _bookingService.GenerateNextBookingReferenceAsync();

            // Assert
            Assert.IsNotNull(bookingReference);
            Assert.AreEqual(expectedBookingReference, bookingReference);
        }

        [TestMethod]
        public async Task GenerateNextBookingReference_NoHolidayBookingsExistInDatabase_ReturnsFirstBookingReference()
        {
            // Arrange
            var expectedBookingReference = "G0001";
            
            _mockHolidayBookingRepository.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<HolidayBooking>());

            // Act
            var bookingReference = await _bookingService.GenerateNextBookingReferenceAsync();

            // Assert
            Assert.IsNotNull(bookingReference);
            Assert.AreEqual(expectedBookingReference, bookingReference);
        }
    }
}
