using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gotorz.Server.Services;
using Gotorz.Server.Contexts;
using Gotorz.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gotorz.Server.UnitTests.Services
{
    [TestClass]
    public class HotelBookingServiceTests
    {
        private HotelBookingService _service;
        private GotorzDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GotorzDbContext>()
                .UseInMemoryDatabase(databaseName: "HotelBookingTestDb")
                .Options;

            _context = new GotorzDbContext(options);
            _service = new HotelBookingService(_context);
        }

        [TestMethod]
        public async Task AddHotelBookingAsync_Should_AddBookingToDatabase()
        {
            // Arrange
            var booking = new HotelBooking
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                HotelId = 1,
                HotelRoomId = 1,
                Price = 500.00m,
                RoomCapacity = 2
            };

            // Act
            await _service.AddHotelBookingAsync(booking);
            var savedBooking = await _context.HotelBookings.FirstOrDefaultAsync();

            // Assert
            Assert.IsNotNull(savedBooking);
            Assert.AreEqual(1, savedBooking.HotelId);
            Assert.AreEqual(2, savedBooking.RoomCapacity);
        }
    }
}