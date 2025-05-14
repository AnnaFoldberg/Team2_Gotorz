using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Gotorz.Server.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Gotorz.Shared.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Gotorz.Server.UnitTests.Controllers
{
    /// <summary>
    /// Contains unit tests for the <see cref="BookingController"/> class.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class BookingControllerTests
    {
        private BookingController _bookingController;
        private Mock<IMapper> _mockMapper;
        private Mock<IBookingService> _mockBookingService;
        private Mock<IRepository<HolidayPackage>> _mockHolidayPackageRepository;
        private Mock<IHolidayBookingRepository> _mockHolidayBookingRepository;
        private Mock<IRepository<Traveller>> _mockTravellerRepository;
        private Mock<IUserRepository> _mockUserRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMapper = new Mock<IMapper>();
            _mockBookingService = new Mock<IBookingService>();
            _mockHolidayPackageRepository = new Mock<IRepository<HolidayPackage>>();
            _mockHolidayBookingRepository = new Mock<IHolidayBookingRepository>();
            _mockTravellerRepository = new Mock<IRepository<Traveller>>();
            _mockUserRepository = new Mock<IUserRepository>();

            _bookingController = new BookingController(_mockMapper.Object, _mockBookingService.Object,
                _mockHolidayPackageRepository.Object, _mockHolidayBookingRepository.Object,
                _mockTravellerRepository.Object, _mockUserRepository.Object);
        }

        // -------------------- GetNextBookingReferenceAsync --------------------
        [TestMethod]
        public async Task GetNextBookingReferenceAsync_ReturnsNextBookingReference()
        {
            // Arrange
            var expectedBookingReference = "G0002";
            
            _mockBookingService.Setup(s => s.GenerateNextBookingReferenceAsync()).ReturnsAsync(expectedBookingReference);

            // Act
            var bookingReference = await _bookingController.GetNextBookingReferenceAsync();

            // Assert
            Assert.IsNotNull(bookingReference);
            Assert.AreEqual(expectedBookingReference, bookingReference);
            _mockBookingService.Verify(s => s.GenerateNextBookingReferenceAsync(), Times.Once);
        }

        // -------------------- GetHolidayBookingAsync --------------------
        [TestMethod]
        public async Task GetHolidayBookingAsync_ValidBookingReference_ReturnsHolidayBooking()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackageDto.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(mockHolidayBookingDto.Customer.UserId)).ReturnsAsync(mockHolidayBooking.Customer);
            _mockMapper.Setup(m => m.Map<HolidayBookingDto>(mockHolidayBooking)).Returns(mockHolidayBookingDto);

            // Act
            var holidayBooking = await _bookingController.GetHolidayBookingAsync(bookingReference);

            // Assert
            Assert.IsNotNull(holidayBooking);
            Assert.AreSame(mockHolidayBookingDto, holidayBooking);
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockMapper.Verify(m => m.Map<HolidayBookingDto>(mockHolidayBooking), Times.Once);
        }

        [TestMethod]
        public async Task GetHolidayBookingAsync_InvalidBookingReference_ReturnsNull()
        {
            // Arrange
            var bookingReference = "G01";

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var holidayBooking = await _bookingController.GetHolidayBookingAsync(bookingReference);

            // Assert
            Assert.IsNull(holidayBooking);
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
        }

        // -------------------- PatchHolidayBookingStatusAsync --------------------
        [TestMethod]
        public async Task PatchHolidayBookingStatusAsync_ValidHolidayBooking_UpdatesHolidayBookingStatusAndReturnsOk()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };
            
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackage.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockHolidayBookingRepository.Setup(r => r.UpdateAsync(mockHolidayBooking)).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingController.PatchHolidayBookingStatusAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully updated holiday booking {mockHolidayBooking.BookingReference}", okResult.Value);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.UpdateAsync(mockHolidayBooking), Times.Once);
        }

        [TestMethod]
        public async Task PatchHolidayBookingStatusAsync_InvalidHolidayBooking_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };
            
            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Customer = mockCustomerDto,
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var result = await _bookingController.PatchHolidayBookingStatusAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"The holiday booking does not exist in the database", badRequestResult.Value);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.UpdateAsync(It.IsAny<HolidayBooking>()), Times.Never);
        }

        [TestMethod]
        public async Task PatchHolidayBookingStatusAsync_NullHolidayBooking_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            // Act
            var result = await _bookingController.PatchHolidayBookingStatusAsync((HolidayBookingDto)null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No holiday booking was provided", badRequestResult.Value);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Never);
            _mockHolidayBookingRepository.Verify(s => s.UpdateAsync(It.IsAny<HolidayBooking>()), Times.Never);
        }

        // -------------------- PostHolidayBookingAsync --------------------
        [TestMethod]
        public async Task PostHolidayBookingAsync_ValidHolidayBooking_AddsHolidayBookingAndReturnsOk()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "customer@mail.com"),
                new Claim(ClaimTypes.NameIdentifier, "Customer"),
                new Claim(ClaimTypes.Role, "customer")
            }, "mock"));

            _bookingController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = mockHolidayPackage.HolidayPackageId,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackage.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockMapper.Setup(m => m.Map<HolidayBooking>(mockHolidayBookingDto)).Returns(mockHolidayBooking);
            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(mockCustomerDto.UserId)).ReturnsAsync(mockCustomer);
            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);
            _mockHolidayBookingRepository.Setup(r => r.AddAsync(mockHolidayBooking)).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingController.PostHolidayBookingAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added holiday booking {mockHolidayBooking.BookingReference} to database", okResult.Value);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.AddAsync(mockHolidayBooking), Times.Once);
        }

        [TestMethod]
        public async Task PostHolidayBookingAsync_HolidayBookingAlreadyExistsInDatabase_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackageDto.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var result = await _bookingController.PostHolidayBookingAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"A holiday booking with the same booking reference already exists in the database", badRequestResult.Value);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
        }

        [TestMethod]
        public async Task PostHolidayBookingAsync_NullHolidayBooking_ReturnsBadRequest()
        {
            // Act
            var result = await _bookingController.PostHolidayBookingAsync(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No holiday booking was provided", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostHolidayBookingAsync_InvalidHolidayPackage_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Venice",
                Description = "Trip",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 2,
                Title = "Rome",
                Description = "Trip",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = 2
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockMapper.Setup(m => m.Map<HolidayBooking>(mockHolidayBookingDto)).Returns(mockHolidayBooking);
            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });
            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var result = await _bookingController.PostHolidayBookingAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"Holiday package linked to booking does not exist", badRequestResult.Value);
            _mockHolidayPackageRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.AddAsync(mockHolidayBooking), Times.Never);
        }

        [TestMethod]
        public async Task PostHolidayBookingAsync_InvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackage = new HolidayPackage
            {
                HolidayPackageId = 1,
                Title = "Venice",
                MaxCapacity = 2
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Venice",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = 2
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            _mockMapper.Setup(m => m.Map<HolidayBooking>(mockHolidayBookingDto)).Returns(mockHolidayBooking);
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(mockCustomerDto.UserId)).ReturnsAsync((ApplicationUser)null);
            _mockHolidayPackageRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<HolidayPackage> { mockHolidayPackage });
            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var result = await _bookingController.PostHolidayBookingAsync(mockHolidayBookingDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"Customer linked to booking does not exist", badRequestResult.Value);
            _mockHolidayPackageRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockUserRepository.Verify(s => s.GetUserByIdAsync(mockCustomerDto.UserId), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockHolidayBookingRepository.Verify(s => s.AddAsync(mockHolidayBooking), Times.Never);
        }

        // -------------------- GetTravellersAsync --------------------
        [TestMethod]
        public async Task GetTravellersAsync_ValidBookingReference_ReturnsTravellersMatchingBookingReference()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackageDto.HolidayPackageId
            };

            var mockHolidayBookingTwo = new HolidayBooking
            {
                HolidayBookingId = 2,
                BookingReference = "G02",
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackageDto.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            var mockHolidayBookingDtoTwo = new HolidayBookingDto
            {
                BookingReference = "G02",
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            var mockTravellers = new List<Traveller>
            {
                new Traveller { TravellerId = 1, Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBookingId = mockHolidayBooking.HolidayBookingId},
                new Traveller { TravellerId = 2, Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBookingId = mockHolidayBooking.HolidayBookingId},
                new Traveller { TravellerId = 3, Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBookingId = mockHolidayBooking.HolidayBookingId},
                new Traveller { TravellerId = 4, Name = "Traveller 4", Age = 24, PassportNumber = "P4", HolidayBookingId = mockHolidayBookingTwo.HolidayBookingId}
            };

            var mockTravellerDtos = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 4", Age = 24, PassportNumber = "P4", HolidayBooking = mockHolidayBookingDtoTwo}
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockTravellerRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(mockTravellers);

            var filteredTravellers = mockTravellers.Where(t => t.HolidayBookingId == mockHolidayBooking.HolidayBookingId);
            
            _mockMapper.Setup(m => m.Map<IEnumerable<TravellerDto>>(filteredTravellers)).Returns(mockTravellerDtos.Take(3));

            // Act
            var travellers = await _bookingController.GetTravellersAsync(bookingReference);

            // Assert
            Assert.IsNotNull(travellers);
            Assert.AreEqual(3, travellers.Count());
            CollectionAssert.AreEqual(mockTravellerDtos.Take(3).ToList(), travellers.ToList());
            CollectionAssert.DoesNotContain(mockTravellerDtos.TakeLast(1).ToList(), travellers.ToList());
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockTravellerRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<TravellerDto>>(filteredTravellers), Times.Once);
        }

        [TestMethod]
        public async Task GetTravellersAsync_InvalidBookingReference_ReturnsNull()
        {
            // Arrange
            var bookingReference = "G01";

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var travellers = await _bookingController.GetTravellersAsync(bookingReference);

            // Assert
            Assert.IsNull(travellers);
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockTravellerRepository.Verify(r => r.GetAllAsync(), Times.Never);
        }


        // -------------------- PostTravellersAsync --------------------
        [TestMethod]
        public async Task PostTravellersAsync_ValidTravellers_AddsTravellersAndReturnsOk()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomer = new ApplicationUser
            {
                Id = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBooking
            {
                HolidayBookingId = 1,
                BookingReference = bookingReference,
                Status = 0,
                CustomerId = mockCustomer.Id,
                Customer = mockCustomer,
                HolidayPackageId = mockHolidayPackageDto.HolidayPackageId
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            var mockTravellers = new List<Traveller>
            {
                new Traveller { TravellerId = 1, Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBookingId = mockHolidayBooking.HolidayBookingId},
                new Traveller { TravellerId = 2, Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBookingId = mockHolidayBooking.HolidayBookingId},
                new Traveller { TravellerId = 3, Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBookingId = mockHolidayBooking.HolidayBookingId}
            };

            var mockTravellerDtos = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBookingDto}
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockTravellerRepository.Setup(r => r.AddAsync(It.IsAny<Traveller>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<Traveller>(mockTravellerDtos[0])).Returns(mockTravellers[0]);
            _mockMapper.Setup(m => m.Map<Traveller>(mockTravellerDtos[1])).Returns(mockTravellers[1]);
            _mockMapper.Setup(m => m.Map<Traveller>(mockTravellerDtos[2])).Returns(mockTravellers[2]);

            // Act
            var result = await _bookingController.PostTravellersAsync(mockTravellerDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual($"Successfully added 3 traveller(s) to database", okResult.Value);
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
            _mockTravellerRepository.Verify(r => r.AddAsync(It.IsAny<Traveller>()), Times.Exactly(3));
            _mockMapper.Verify(m => m.Map<Traveller>(mockTravellerDtos[0]), Times.Once);
            _mockMapper.Verify(m => m.Map<Traveller>(mockTravellerDtos[1]), Times.Once);
            _mockMapper.Verify(m => m.Map<Traveller>(mockTravellerDtos[2]), Times.Once);
        }

        [TestMethod]
        public async Task PostTravellersAsync_NoMatchingHolidayBooking_ReturnsBadRequest()
        {
            // Arrange
            var bookingReference = "G01";

            var mockCustomerDto = new UserDto
            {
                UserId = "17506e3e-43fd-4152-ae92-1872ddc91aa0"
            };

            var mockHolidayPackageDto = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBookingDto = new HolidayBookingDto
            {
                BookingReference = bookingReference,
                Status = BookingStatus.Pending,
                Customer = mockCustomerDto,
                HolidayPackage = mockHolidayPackageDto
            };

            var mockTravellerDtos = new List<TravellerDto>
            {
                new TravellerDto { Name = "Traveller 1", Age = 21, PassportNumber = "P1", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 2", Age = 22, PassportNumber = "P2", HolidayBooking = mockHolidayBookingDto},
                new TravellerDto { Name = "Traveller 3", Age = 23, PassportNumber = "P3", HolidayBooking = mockHolidayBookingDto}
            };

            _mockHolidayBookingRepository.Setup(r => r.GetByBookingReferenceAsync(bookingReference)).ReturnsAsync((HolidayBooking)null);

            // Act
            var result = await _bookingController.PostTravellersAsync(mockTravellerDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No holiday booking found for booking reference '{bookingReference}'", badRequestResult.Value);
            _mockHolidayBookingRepository.Verify(r => r.GetByBookingReferenceAsync(bookingReference), Times.Once);
        }

        [TestMethod]
        public async Task PostTravellersAsync_EmptyTravellerCollection_ReturnsBadRequest()
        {
            // Arrange
            var mockTravellerDtos = new List<TravellerDto>();

            // Act
            var result = await _bookingController.PostTravellersAsync(mockTravellerDtos);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No travellers were provided", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostTravellersAsync_NullTravellerCollection_ReturnsBadRequest()
        {
            // Act
            var result = await _bookingController.PostTravellersAsync(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual($"No travellers were provided", badRequestResult.Value);
        }
    }
}