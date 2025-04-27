using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Server.Controllers;
using Gotorz.Server.DataAccess;
using Gotorz.Server.Models;
using Gotorz.Shared.DTOs;
using Gotorz.Server.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
        private Mock<IRepository<HolidayPackage>> _mockHolidayPackageRepository;
        private Mock<IHolidayBookingRepository> _mockHolidayBookingRepository;
        private Mock<IRepository<Traveller>> _mockTravellerRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMapper = new Mock<IMapper>();
            _mockHolidayPackageRepository = new Mock<IRepository<HolidayPackage>>();
            _mockHolidayBookingRepository = new Mock<IHolidayBookingRepository>();
            _mockTravellerRepository = new Mock<IRepository<Traveller>>();

            _bookingController = new BookingController(_mockMapper.Object, _mockHolidayPackageRepository.Object,
                _mockHolidayBookingRepository.Object, _mockTravellerRepository.Object);
        }

        // -------------------- GetHolidayBookingAsync --------------------

        // -------------------- PostHolidayBookingAsync --------------------

        // -------------------- GetTravellersAsync --------------------

        // -------------------- PostTravellersAsync --------------------
    }
}