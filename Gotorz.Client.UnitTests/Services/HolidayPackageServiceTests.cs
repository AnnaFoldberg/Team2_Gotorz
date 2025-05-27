using System.Net;
using Moq;
using Moq.Protected;
using Gotorz.Client.Services;
using Gotorz.Shared.DTOs;
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
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new HolidayPackageDto())
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

        [TestMethod]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfHolidayPackages()
        {
            // Arrange
            var expected = new List<HolidayPackageDto>
    {
        new HolidayPackageDto { HolidayPackageId = 1, Title = "Test Package" },
        new HolidayPackageDto { HolidayPackageId = 2, Title = "Another Package" }
    };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(expected)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.AbsolutePath == "/HolidayPackage"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Test Package", result[0].Title);
        }
    }
}

