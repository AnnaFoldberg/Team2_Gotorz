using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Gotorz.Server.Services;
using Gotorz.Server.Contexts;
using Gotorz.Shared.DTOs;
using Gotorz.Server.Models;
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

            // Ensure database is clean before each test
            _context.HotelBookings.RemoveRange(_context.HotelBookings);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task AddHotelBookingAsync_Should_AddBookingToDatabase()
        {
            // Arrange
            var booking = new HotelBooking
            {
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                HotelRoomId = 1,
                HolidayPackageId = 10
            };

            // Act
            await _service.AddHotelBookingAsync(booking);
            var savedBooking = await _context.HotelBookings.FirstOrDefaultAsync();

            // Assert
            Assert.IsNotNull(savedBooking);
            Assert.AreEqual(1, savedBooking.HotelRoomId);
            Assert.AreEqual(2, savedBooking.HolidayPackageId);
        }
    }
}