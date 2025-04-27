
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

        private void SetupResponse(CurrentUserDto? user)
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
            var user = new CurrentUserDto { Email = "test@example.com", IsAuthenticated = true };
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
            var user = new CurrentUserDto { IsAuthenticated = true };
            SetupResponse(user);

            var result = await _service.IsLoggedInAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsLoggedInAsync_UnauthenticatedUser_ReturnsFalse()
        {
            var user = new CurrentUserDto { IsAuthenticated = false };
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
            {
                Claims = new List<ClaimDto>
        {
            new ClaimDto { Type = ClaimTypes.Role, Value = "manager" } // no match
        }
            };
            SetupResponse(user);

            var result = await _service.IsUserInRoleAsync("admin");

            Assert.IsFalse(result); // .Any(...) returns false
        }

        [TestMethod]
        public async Task GetUserIdAsync_ReturnsClaimValue()
        {
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto { FirstName = "Alice", Email = "alice@example.com" };
            SetupResponse(user);

            var result = await _service.GetFirstNameAsync();

            Assert.AreEqual("Alice", result);
        }

        [TestMethod]
        public async Task GetFirstNameAsync_ReturnsEmailAsFallback()
        {
            var user = new CurrentUserDto { FirstName = null, Email = "alice@example.com" };
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
            var user = new CurrentUserDto
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
            var user = new CurrentUserDto { LastName = "Doe" };
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
            var user = new CurrentUserDto { Email = "test@example.com" };
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
    }
}
