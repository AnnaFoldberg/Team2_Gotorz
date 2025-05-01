using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gotorz.Client.Pages;
using Moq;
using Gotorz.Client.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Gotorz.Shared.DTOs;
using Bunit.TestDoubles;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Gotorz.Client.Pages.Booking;
using Gotorz.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace Gotorz.Client.UnitTests.Pages
{
    /// <summary>
    /// Contains unit tests for the <see cref="Travellers"/> page.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class TravellersTests : Bunit.TestContext
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<Travellers>> logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockUserService = new Mock<IUserService>();
            logger = new Mock<ILogger<Travellers>>();

            Services.AddOptions();

            Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>();
                return new DefaultAuthorizationPolicyProvider(options);
            });

            var mockAuthService = new Mock<IAuthorizationService>();
            mockAuthService
                .Setup(x => x.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success());

            Services.AddSingleton(mockAuthService.Object);
            Services.AddSingleton(_mockBookingService.Object);
            Services.AddSingleton(_mockUserService.Object);
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();
            Services.AddSingleton(logger.Object);
        }

        // -------------------- AuthorizeView --------------------
        [TestMethod]
        public void OnInitializedAsync_IsAuthorizedCustomer_ShowsTravellersForm()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains($"Travellers"));
            Assert.IsTrue(component.Markup.Contains($"PassportNumber"));
        }

        [TestMethod]
        public void OnInitializedAsync_IsUnauthorizedCustomer_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("unauthorizedcustomer@mail.com");

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_IsSales_ShowsPageNotFound()
        {
            // Arrange
            SetUser("sales");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_IsAdmin_ShowsPageNotFound()
        {
            // Arrange
            SetUser("admin");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public void OnInitializedAsync_NotLoggedIn_ShowsLogin()
        {
            // Arrange
            SetUser(null);

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Login"));
            Assert.IsFalse(component.Markup.Contains("Page not found."));
            Assert.IsFalse(component.Markup.Contains("Loading ..."));
            Assert.IsFalse(component.Markup.Contains("Travellers"));
            Assert.IsFalse(component.Markup.Contains("Passport Number"));
        }

        // -------------------- OnInitializedAsync --------------------
        [TestMethod]
        public void OnInitializedAsync_HolidayBookingExists_ShowsTravellersFormWithOneTraveller()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Travellers"));
            Assert.IsTrue(component.Markup.Contains("Name"));
            Assert.IsTrue(component.Markup.Contains("Age"));
            Assert.IsTrue(component.Markup.Contains("Passport Number"));
            var editForm = component.Find("form");
            Assert.IsNotNull(editForm);
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(3, inputFields.Count());
            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            var removeButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(addButton);
            Assert.IsNotNull(removeButton);
            Assert.IsTrue(removeButton.HasAttribute("disabled"));
            Assert.IsTrue(addButton.TextContent.Contains("Add traveller"));
            _mockUserService.Verify(s => s.GetEmailAsync(), Times.Once);
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference), Times.Once());
        }

        [TestMethod]
        public void OnInitializedAsync_NullHolidayBooking_ShowsPageNotFound()
        {
            // Arrange
            SetUser("customer");

            var bookingReference = "G01";

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(bookingReference)).ReturnsAsync((HolidayBookingDto)null);

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, null));

            // Assert
            Assert.IsTrue(component.Markup.Contains("Page not found."));
        }

        [TestMethod]
        public async Task OnInitializedAsync_ThrowsException_LogsError()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            // Act
            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            // Assert
            _mockBookingService.Verify(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error initializing travellers")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // -------------------- AddTraveller --------------------
        [TestMethod]
        public void AddTraveller_BelowMaxCapacity_AddsTraveller()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 4
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));

            // Act
            addButton!.Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(6, inputFields.Count());
            Assert.IsNotNull(addButton);
            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            foreach (var removeButton in removeButtons)
                Assert.IsFalse(removeButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void AddTraveller_BelowToEqualToMaxCapacity_AddsTravellers()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 4
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));

            // Act
            addButton!.Click();
            addButton!.Click();
            addButton!.Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(12, inputFields.Count());
            Assert.IsNotNull(addButton);
            addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            Assert.IsTrue(addButton.HasAttribute("disabled"));
            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            foreach (var removeButton in removeButtons)
                Assert.IsFalse(removeButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void AddTraveller_BelowToOAboveMaxCapacity_AddsTravellersUpToMaxCapacity()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));

            // Act
            addButton!.Click();
            addButton!.Click();
            addButton!.Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(6, inputFields.Count());
            Assert.IsNotNull(addButton);
            addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            Assert.IsTrue(addButton.HasAttribute("disabled"));
            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            foreach (var removeButton in removeButtons)
                Assert.IsFalse(removeButton.HasAttribute("disabled"));
        }

        // -------------------- RemoveTraveller --------------------
        [TestMethod]
        public void RemoveTraveller_MoreThanOneTraveller_RemovesTraveller()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 4
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();
            addButton!.Click();

            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            
            // Act
            removeButtons.First().Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(6, inputFields.Count());
            Assert.IsNotNull(addButton);
            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            foreach (var removeButton in removeButtons)
                Assert.IsFalse(removeButton.HasAttribute("disabled"));
        }

        [TestMethod]
        public void RemoveTraveller_MoreThanOneToOneTraveller_RemovesTravellers()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 4
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();
            addButton!.Click();
            
            // Act
            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.First().Click();

            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.First().Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(3, inputFields.Count());
            Assert.IsNotNull(addButton);
            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            Assert.AreEqual(1, removeButtons.Count());
            Assert.IsTrue(removeButtons.First().HasAttribute("disabled"));
        }

        [TestMethod]
        public void RemoveTraveller_MoreThanOneToNegativeTravellers_RemovesTravellersDownToOneTraveller()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 4
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            // Act
            buttons = component.FindAll("button");
            var removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.First().Click();

            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.First().Click();

            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.First().Click();

            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            removeButtons.Last().Click();

            // Assert
            var inputFields = component.FindAll("div.form-group");
            Assert.AreEqual(3, inputFields.Count());
            Assert.IsNotNull(addButton);
            buttons = component.FindAll("button");
            removeButtons = buttons.Where(b => b.InnerHtml.Contains("bi-dash-square-fill"));
            Assert.IsNotNull(removeButtons);
            Assert.AreEqual(1, removeButtons.Count());
            Assert.IsTrue(removeButtons.First().HasAttribute("disabled"));
        }

        // -------------------- ConfirmAndPostTravellersAsync --------------------
        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_ValidForm_CallsPostTravellersAsync()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.Is<List<TravellerDto>>(travellers => travellers.Count == 2 &&
                travellers[0].Name == "Traveller 1" && travellers[0].Age == 21 && travellers[0].PassportNumber == "P1" &&
                travellers[1].Name == "Traveller 2" && travellers[1].Age == 22 && travellers[1].PassportNumber == "P2")), Times.Once);
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains("/booking/request-confirmation/G01"));
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_AgeIsZero_CallsPostTravellersAsync()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("0");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.Is<List<TravellerDto>>(travellers => travellers.Count == 2 &&
                travellers[0].Name == "Traveller 1" && travellers[0].Age == 0 && travellers[0].PassportNumber == "P1" &&
                travellers[1].Name == "Traveller 2" && travellers[1].Age == 22 && travellers[1].PassportNumber == "P2")), Times.Once);
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains("/booking/request-confirmation/G01"));
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_AgeIs120_CallsPostTravellersAsync()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("120");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.Is<List<TravellerDto>>(travellers => travellers.Count == 2 &&
                travellers[0].Name == "Traveller 1" && travellers[0].Age == 120 && travellers[0].PassportNumber == "P1" &&
                travellers[1].Name == "Traveller 2" && travellers[1].Age == 22 && travellers[1].PassportNumber == "P2")), Times.Once);
            Assert.IsTrue(Services.GetRequiredService<NavigationManager>().Uri.Contains("/booking/request-confirmation/G01"));
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_AgeIsNegative_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("-1");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_AgeIsOver120_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("121");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Never());
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_NameIsEmpty_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Never());
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_NameIsWhiteSpace_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("       ");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Never());
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_PassportNumberIsEmpty_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Never());
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_PassportNumberIsWhiteSpace_ShowsCustomErrorMessage()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("    ");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            Assert.IsTrue(component.Markup.Contains("One or more fields were not filled in properly."));
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Never());
        }

        [TestMethod]
        public async Task ConfirmAndPostTravellersAsync_ThrowsException_LogsError()
        {
            // Arrange
            SetUser("customer");

            var mockHolidayPackage = new HolidayPackageDto
            {
                HolidayPackageId = 1,
                Title = "Rome",
                MaxCapacity = 2
            };

            var mockHolidayBooking = new HolidayBookingDto
            {
                BookingReference = "G01",
                CustomerEmail = "customer@mail.com",
                Status = BookingStatus.Pending,
                HolidayPackage = mockHolidayPackage
            };

            _mockUserService.Setup(s => s.GetEmailAsync()).ReturnsAsync("customer@mail.com");
            _mockBookingService.Setup(s => s.GetHolidayBookingAsync(mockHolidayBooking.BookingReference)).ReturnsAsync(mockHolidayBooking);
            _mockBookingService.Setup(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()))
                .ThrowsAsync(new Exception("Mocked booking service failure"));

            var component = RenderComponent<Travellers>(parameters => parameters.Add(c => c.BookingReference, mockHolidayBooking.BookingReference));

            var addButton = component.FindAll("button").FirstOrDefault(b => b.InnerHtml.Contains("bi-plus-square-fill"));
            addButton!.Click();

            component.Find("#name-0").Change("Traveller 1");
            component.Find("#age-0").Change("21");
            component.Find("#passportNumber-0").Change("P1");
            component.Find("#name-1").Change("Traveller 2");
            component.Find("#age-1").Change("22");
            component.Find("#passportNumber-1").Change("P2");

            // Act
            component.Find("form").Submit();

            // Assert
            _mockBookingService.Verify(s => s.PostTravellersAsync(It.IsAny<List<TravellerDto>>()), Times.Once());
            
            // Structure from ChatGPT. Customized for this project.
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Error committing travellers")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // -------------------- Helper Methods --------------------
        private void SetUser(string? role)
        {
            var authContext = this.AddTestAuthorization();
            if (role == null)
            {
                authContext.SetNotAuthorized();
                return;
            }

            authContext.SetAuthorized("Test user");
            authContext.SetClaims(
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.Role, role)
            );
            _mockUserService.Setup(s => s.IsUserInRoleAsync(role)).ReturnsAsync(true);
        }
    }
}