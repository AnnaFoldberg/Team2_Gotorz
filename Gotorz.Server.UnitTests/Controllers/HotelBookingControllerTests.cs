using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Server.Controllers;
using Gotorz.Server.Services;
using Gotorz.Shared.DTOs;
using Gotorz.Server.Models;
using Gotorz.Server.DataAccess;
using AutoMapper;


namespace Gotorz.Server.UnitTests.Controllers
{
    [TestClass]
public class HotelBookingControllerTests
{
    private Mock<IHotelBookingService> _mockService = null!;
    private Mock<IHolidayPackageRepository> _mockHolidayRepo = null!;
    private Mock<IHotelRoomRepository> _mockRoomRepo = null!;
    private Mock<IHotelBookingRepository> _mockBookingRepo = null!;
    private Mock<IHotelRepository> _mockHotelRepo = null!;
    private Mock<IMapper> _mockMapper = null!;
    private HotelBookingController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IHotelBookingService>();
        _mockHolidayRepo = new Mock<IHolidayPackageRepository>();
        _mockRoomRepo = new Mock<IHotelRoomRepository>();
        _mockBookingRepo = new Mock<IHotelBookingRepository>();
        _mockHotelRepo = new Mock<IHotelRepository>();
        _mockMapper = new Mock<IMapper>();

        _controller = new HotelBookingController(
            _mockService.Object,
            _mockHolidayRepo.Object,
            _mockBookingRepo.Object,
            _mockHotelRepo.Object,
            _mockRoomRepo.Object,
            _mockMapper.Object
        );
    }

    [TestMethod]
    public async Task AddBooking_ValidInput_ReturnsOk()
    {
        // Arrange
        var bookingDto = new HotelBookingDto
        {
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(1),
            HolidayPackageDto = new HolidayPackageDto { URL = "test-url", HolidayPackageId = 1 },
            HotelRoom = new HotelRoomDto { ExternalRoomId = "room-123", HotelRoomId = 1 }
        };

        var mappedBooking = new HotelBooking
        {
            HotelRoom = new HotelRoom { ExternalRoomId = "room-123" },
            HolidayPackage = new HolidayPackage()
        };

        _mockMapper.Setup(m => m.Map<HotelBooking>(bookingDto)).Returns(mappedBooking);
        _mockRoomRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HotelRoom>
        {
            new HotelRoom { ExternalRoomId = "room-123", HotelRoomId = 1 }
        });

        _mockHolidayRepo.Setup(h => h.GetByUrlAsync("test-url")).ReturnsAsync(new HolidayPackage
        {
            HolidayPackageId = 1
        });

        // Act
        var result = await _controller.AddBooking(bookingDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockService.Verify(s => s.AddHotelBookingAsync(It.IsAny<HotelBooking>()), Times.Once);
    }
}
}