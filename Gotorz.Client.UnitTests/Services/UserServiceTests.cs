
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Gotorz.Client.Services;
using Gotorz.Shared.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Security.Claims;

namespace Gotorz.Client.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="UserService"/> class.
    /// </summary>
    /// <author>Eske</author>
    [TestClass]
    public class UserServiceTests
    {
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _handler;
        private UserService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _handler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _service = new UserService(_httpClient);
        }

        private void SetupResponse(UserDto? user)
        {
            var json = JsonSerializer.Serialize(user);
            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ReturnsUser()
        {
            var user = new UserDto { Email = "test@example.com", IsAuthenticated = true };
            SetupResponse(user);

            var result = await _service.GetCurrentUserAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.IsTrue(result.IsAuthenticated);
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ReturnsNullOnError()
        {
            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var result = await _service.GetCurrentUserAsync();

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task IsLoggedInAsync_AuthenticatedUser_ReturnsTrue()
        {
            var user = new UserDto { IsAuthenticated = true };
            SetupResponse(user);

            var result = await _service.IsLoggedInAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsLoggedInAsync_UnauthenticatedUser_ReturnsFalse()
        {
            var user = new UserDto { IsAuthenticated = false };
            SetupResponse(user);

            var result = await _service.IsLoggedInAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsLoggedInAsync_NullUser_ReturnsFalse()
        {
            // Simulate backend failure or unauthenticated user
            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

            var result = await _service.IsLoggedInAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_RoleExists_ReturnsTrue()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
                {
                    new ClaimDto { Type = ClaimTypes.Role, Value = "admin" }
                }
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_RoleMissing_ReturnsFalse()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
                {
                    new ClaimDto { Type = ClaimTypes.Role, Value = "sales" }
                }
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_NullUser_ReturnsFalse()
        {
            SetupResponse(null);
            var result = await _service.IsUserInRoleAsync("admin");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_ClaimNotFound_ReturnsFalse()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "user" }
        }
            };
            SetupResponse(user);
            var result = await _service.IsUserInRoleAsync("admin");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_ClaimsNull_ReturnsFalse()
        {
            var user = new UserDto
            {
                Claims = null
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_NoMatchingRole_ReturnsFalse()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "user" }
        }
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsUserInRoleAsync_ClaimsExist_NoMatch_ReturnsFalse()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "manager" } // no match
        }
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetUserIdAsync_ReturnsClaimValue()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
                {
                    new ClaimDto { Type = ClaimTypes.NameIdentifier, Value = "user123" }
                }
            };
            SetupResponse(user);

            var result = await _service.GetUserIdAsync();

            Assert.AreEqual("user123", result);
        }

        [TestMethod]
        public async Task GetUserIdAsync_NullUser_ReturnsNull()
        {
            SetupResponse(null);
            var result = await _service.GetUserIdAsync();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserIdAsync_ClaimMissing_ReturnsNull()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "admin" }
        }
            };
            SetupResponse(user);
            var result = await _service.GetUserIdAsync();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserIdAsync_ClaimsNull_ReturnsNull()
        {
            var user = new UserDto
            {
                Claims = null
            };
            SetupResponse(user);

            var result = await _service.GetUserIdAsync();

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetFirstNameAsync_ReturnsFirstName()
        {
            var user = new UserDto { FirstName = "Alice", Email = "alice@example.com" };
            SetupResponse(user);

            var result = await _service.GetFirstNameAsync();

            Assert.AreEqual("Alice", result);
        }

        [TestMethod]
        public async Task GetFirstNameAsync_ReturnsEmailAsFallback()
        {
            var user = new UserDto { FirstName = null, Email = "alice@example.com" };
            SetupResponse(user);

            var result = await _service.GetFirstNameAsync();

            Assert.AreEqual("alice@example.com", result);
        }

        [TestMethod]
        public async Task GetFirstNameAsync_NullUser_ReturnsNull()
        {
            SetupResponse(null);
            var result = await _service.GetFirstNameAsync();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetFirstNameAsync_FirstNameMissing_EmailExists_ReturnsEmail()
        {
            var user = new UserDto
            {
                FirstName = null,
                Email = "fallback@example.com"
            };
            SetupResponse(user);
            var result = await _service.GetFirstNameAsync();
            Assert.AreEqual("fallback@example.com", result);
        }

        [TestMethod]
        public async Task GetLastNameAsync_ReturnsLastName()
        {
            var user = new UserDto { LastName = "Doe" };
            SetupResponse(user);

            var result = await _service.GetLastNameAsync();

            Assert.AreEqual("Doe", result);
        }

        [TestMethod]
        public async Task GetLastNameAsync_NullUser_ReturnsNull()
        {
            SetupResponse(null);
            var result = await _service.GetLastNameAsync();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetEmailAsync_ReturnsEmail()
        {
            var user = new UserDto { Email = "test@example.com" };
            SetupResponse(user);

            var result = await _service.GetEmailAsync();

            Assert.AreEqual("test@example.com", result);
        }

        [TestMethod]
        public async Task GetEmailAsync_NullUser_ReturnsNull()
        {
            SetupResponse(null);
            var result = await _service.GetEmailAsync();
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            var testUser = new UserDto
            {
                Email = "profileuser@example.com",
                FirstName = "Profile",
                LastName = "User",
                IsAuthenticated = true,
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "customer" }
        }
            };

            SetupResponse(testUser);

            var result = await _service.GetUserByIdAsync("some-id");

            Assert.IsNotNull(result);
            Assert.AreEqual("profileuser@example.com", result.Email);
            Assert.AreEqual("Profile", result.FirstName);
            Assert.AreEqual("User", result.LastName);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            SetupResponse(null);

            var result = await _service.GetUserByIdAsync("invalid-id");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsNull_WhenHttpFails()
        {
            // Arrange
            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException()); // <-- simulate failure

            // Act
            var result = await _service.GetUserByIdAsync("invalid-id");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserRoleAsync_UserHasRole_ReturnsRole()
        {
            var user = new UserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "admin" }
        }
            };
            SetupResponse(user); // You already have SetupResponse() helper

            var result = await _service.GetUserRoleAsync();

            Assert.AreEqual("admin", result);
        }

        [TestMethod]
        public async Task GetUserRoleAsync_UserHasNoClaims_ReturnsNull()
        {
            var user = new UserDto
            {
                Claims = null // no claims
            };
            SetupResponse(user);

            var result = await _service.GetUserRoleAsync();

            Assert.IsNull(result);
        }


    }
}
