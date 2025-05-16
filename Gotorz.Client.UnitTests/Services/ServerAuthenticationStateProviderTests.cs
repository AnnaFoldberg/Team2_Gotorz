using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Gotorz.Shared.DTOs;
using Moq;
using Moq.Protected;
using Gotorz.Client.Services;

namespace Gotorz.Client.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="ServerAuthenticationStateProvider"/> class.
    /// </summary>
    /// <author>Eske</author>
    [TestClass]
    public class ServerAuthenticationStateProviderTests
    {
        private const string Endpoint = "https://localhost/api/account/currentuser";

        private static HttpClient CreateMockHttpClient(HttpResponseMessage response)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString() == Endpoint),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            return new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://localhost")
            };
        }

        [TestMethod]
        public async Task GetAuthenticationStateAsync_ReturnsAuthenticatedUserWithClaims()
        {
            // Arrange
            var user = new UserDto
            {
                Email = "test@example.com",
                IsAuthenticated = true,
                Claims = new List<ClaimDto>
                {
                    new ClaimDto { Type = ClaimTypes.Role, Value = "admin" },
                    new ClaimDto { Type = ClaimTypes.Name, Value = "Tester" }
                }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(user))
            };

            var httpClient = CreateMockHttpClient(response);
            var provider = new ServerAuthenticationStateProvider(httpClient);

            // Act
            var authState = await provider.GetAuthenticationStateAsync();
            var principal = authState.User;

            // Assert
            Assert.IsTrue(principal.Identity?.IsAuthenticated);
            Assert.AreEqual("test@example.com", principal.FindFirst(ClaimTypes.Email)?.Value);
            Assert.AreEqual("admin", principal.FindFirst(ClaimTypes.Role)?.Value);
            Assert.AreEqual("Tester", principal.FindFirst(ClaimTypes.Name)?.Value);
        }

        [TestMethod]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyIdentity_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var user = new UserDto
            {
                Email = "test@example.com",
                IsAuthenticated = false,
                Claims = new List<ClaimDto>()
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(user))
            };

            var httpClient = CreateMockHttpClient(response);
            var provider = new ServerAuthenticationStateProvider(httpClient);

            // Act
            var authState = await provider.GetAuthenticationStateAsync();
            var principal = authState.User;

            // Assert
            Assert.IsFalse(principal.Identity?.IsAuthenticated);
            Assert.AreEqual(0, principal.Claims.Count());
        }

        [TestMethod]
        public async Task GetAuthenticationStateAsync_ReturnsEmptyIdentity_OnHttpFailure()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            var httpClient = CreateMockHttpClient(response);
            var provider = new ServerAuthenticationStateProvider(httpClient);

            // Act
            var authState = await provider.GetAuthenticationStateAsync();
            var principal = authState.User;

            // Assert
            Assert.IsFalse(principal.Identity?.IsAuthenticated);
            Assert.AreEqual(0, principal.Claims.Count());
        }
    }
}
