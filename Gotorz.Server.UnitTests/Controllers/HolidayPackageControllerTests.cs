using AutoMapper;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gotorz.Server.UnitTests.Controllers
{
    [TestClass]
    public class HolidayPackageControllerTests
    {
        private HolidayPackageController _controller;
        private Mock<IRepository<HolidayPackage>> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHolidayPackageRepository> _mockHolidayPackageRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<HolidayPackage>>();
            _mockMapper = new Mock<IMapper>();
            _mockHolidayPackageRepository = new Mock<IHolidayPackageRepository>();

            _controller = new HolidayPackageController(
                _mockRepository.Object,
                _mockHolidayPackageRepository.Object,
                _mockMapper.Object);
        }


        // -------------------- Create --------------------
        [TestMethod]
        public async Task Create_ValidDto_ReturnsSuccessMessage()
        {
            // Arrange
            var dto = new HolidayPackageDto(); 
            var mappedPackage = new HolidayPackage();
            var mappedPackageDto = new HolidayPackageDto();

            _mockMapper.Setup(m => m.Map<HolidayPackage>(dto)).Returns(mappedPackage);
            _mockRepository.Setup(r => r.AddAsync(mappedPackage)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<HolidayPackageDto>(mappedPackage)).Returns(mappedPackageDto);


            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            Assert.AreEqual(mappedPackageDto, (result.Result as OkObjectResult)?.Value);
            _mockMapper.Verify(m => m.Map<HolidayPackage>(dto), Times.Once);
            _mockMapper.Verify(m => m.Map<HolidayPackageDto>(mappedPackage), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(mappedPackage), Times.Once);
        }

        // -------------------- GetAll --------------------
        [TestMethod]
        public async Task GetAllAsync_ReturnsOkResult_WithListOfDtos()
        {
            // Arrange
            var holidayPackages = new List<HolidayPackage>
            {
            new HolidayPackage { HolidayPackageId = 1, Title = "Test Package 1" },
             new HolidayPackage { HolidayPackageId = 2, Title = "Test Package 2" }
            };

            var expectedDtos = new List<HolidayPackageDto>
            {
                new HolidayPackageDto { HolidayPackageId = 1, Title = "Test Package 1" },
                new HolidayPackageDto { HolidayPackageId = 2, Title = "Test Package 2" }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(holidayPackages);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<HolidayPackageDto>>(holidayPackages))
                       .Returns(expectedDtos);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualDtos = okResult.Value as IEnumerable<HolidayPackageDto>;
            Assert.IsNotNull(actualDtos);
            Assert.AreEqual(expectedDtos.Count, actualDtos.Count());

            // Verify mocks were called
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<HolidayPackageDto>>(holidayPackages), Times.Once);
        }

    }
}
