using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<HolidayPackage>>();
            _mockMapper = new Mock<IMapper>();

            _controller = new HolidayPackageController(_mockRepository.Object, _mockMapper.Object);
        }


        // -------------------- Create --------------------
        [TestMethod]
        public async Task Create_ValidDto_ReturnsSuccessMessage()
        {
            // Arrange
            var dto = new HolidayPackageDto(); 
            var mappedPackage = new HolidayPackage();

            _mockMapper.Setup(m => m.Map<HolidayPackage>(dto)).Returns(mappedPackage);
            _mockRepository.Setup(r => r.AddAsync(mappedPackage)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var okResult = result as OkObjectResult;
            Assert.AreEqual("Succesfully created package", okResult?.Value);

            _mockMapper.Verify(m => m.Map<HolidayPackage>(dto), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(mappedPackage), Times.Once);
        }
    }
}
