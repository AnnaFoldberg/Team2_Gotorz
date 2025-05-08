using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Gotorz.Client.Services;
using Gotorz.Shared.DTO;
using System.Net.Http.Json;

namespace Gotorz.Client.UnitTests.Services
{
    [TestClass]
    public class HolidayPackageServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HolidayPackageService _service;
        private HttpClient _httpClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost") // Simuleret base-URL
            };

            _service = new HolidayPackageService(_httpClient);
        }

        [TestMethod]
        public async Task CreateAsync_ValidDto_PostsDtoToCorrectEndpoint()
        {
            // Arrange
            var dto = new HolidayPackageDto();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.AbsolutePath == "/HolidayPackage"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            // Act
            await _service.CreateAsync(dto);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.AbsolutePath == "/HolidayPackage"),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}

