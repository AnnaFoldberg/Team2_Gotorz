using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using Gotorz.Server.Controllers;
using Gotorz.Server.Models;
using Gotorz.Server.DataAccess;
using Gotorz.Shared.DTOs;
using Microsoft.AspNetCore.Http;

namespace Gotorz.Server.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for the <see cref="AccountController"/> class.
    /// </summary>
    /// <author>Eske</author>
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<IUserRepository> _userRepoMock = null!;
        private AccountController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _controller = new AccountController(_userRepoMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "user@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Role, "admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var email = "user@example.com";
            var password = "Test123";

            _userRepoMock.Setup(r => r.LoginAsync(email, password))
                         .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _controller.Login(new LoginDto { Email = email, Password = password });

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual("Logged in", ok.Value);
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var email = "user@example.com";
            var password = "WrongPass";

            _userRepoMock.Setup(r => r.LoginAsync(email, password))
                         .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(new LoginDto { Email = email, Password = password });

            var unauthorized = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual("Invalid login", unauthorized.Value);
        }

        [TestMethod]
        public async Task Register_AdminRoleAllowed_ReturnsOk()
        {
            var dto = new RegisterDto
            {
                Email = "admin@example.com",
                Password = "Test123!",
                Role = "sales",
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "12345678"
            };

            _userRepoMock.Setup(r => r.RegisterAsync(
                It.IsAny<ApplicationUser>(),
                dto.Password,
                dto.Role
            )).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(dto);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual("User registered", ok.Value);
        }

        [TestMethod]
        public async Task Register_Failure_ReturnsBadRequest()
        {
            var dto = new RegisterDto
            {
                Email = "fail@example.com",
                Password = "badpass"
            };

            var error = IdentityResult.Failed(new IdentityError { Description = "Failure reason" });

            _userRepoMock.Setup(r => r.RegisterAsync(
                It.IsAny<ApplicationUser>(),
                dto.Password,
                It.IsAny<string>()
            )).ReturnsAsync(error);

            var result = await _controller.Register(dto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetCurrentUser_ReturnsMappedDto()
        {
            _userRepoMock.Setup(r => r.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(new ApplicationUser
                         {
                             UserName = "user@example.com",
                             FirstName = "Eske",
                             LastName = "Knudsen",
                             PhoneNumber = "12345678"
                         });

            var result = await _controller.GetCurrentUser();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var dto = ok.Value as CurrentUserDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual("user@example.com", dto.Email);
            Assert.AreEqual("Eske", dto.FirstName);
            Assert.AreEqual("Knudsen", dto.LastName);
            Assert.AreEqual("12345678", dto.PhoneNumber);
            Assert.IsTrue(dto.IsAuthenticated);
        }

        [TestMethod]
        public async Task Logout_CallsLogoutAsync_AndReturnsOk()
        {
            _userRepoMock.Setup(r => r.LogoutAsync()).Returns(Task.CompletedTask);

            var result = await _controller.Logout();

            _userRepoMock.Verify(r => r.LogoutAsync(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetUserById_UserExists_ReturnsCorrectUserDto()
        {
            // Arrange
            var userId = "test-user-id";
            var testUser = new ApplicationUser
            {
                Id = userId,
                Email = "customer@example.com",
                FirstName = "Test",
                LastName = "Customer",
                PhoneNumber = "87654321"
            };
            var claims = new List<ClaimDto>
            {
                new ClaimDto { Type = ClaimTypes.Role, Value = "customer" }
            };

            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(testUser);
            _userRepoMock.Setup(r => r.GetClaimsAsync(testUser)).ReturnsAsync(claims);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var dto = okResult.Value as CurrentUserDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(testUser.Email, dto.Email);
            Assert.AreEqual(testUser.FirstName, dto.FirstName);
            Assert.AreEqual(testUser.LastName, dto.LastName);
            Assert.AreEqual(testUser.PhoneNumber, dto.PhoneNumber);
            Assert.IsTrue(dto.IsAuthenticated);
            Assert.AreEqual("customer", dto.Claims.FirstOrDefault()?.Value);
        }

        [TestMethod]
        public async Task GetUserById_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = "nonexistent-id";
            _userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

    }
}
