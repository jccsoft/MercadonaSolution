namespace MercadonaAPI.Tests;

public class MercadonaControllerTests
{
    //[Fact]
    //public void GetBaseUri_StandardTest()
    //{
    //    // Arrange
    //    var controller = GetMockMercadonaController();

    //    // Act
    //    var result = controller.GetBaseUri() as ObjectResult;
    //    //int? sc = (HttpStatusCode)result.StatusCode;
    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);

    //    Assert.Equal("https://tienda.mercadona.es/api/categories/", result.Value);
    //}

    //private static MercadonaController GetMockMercadonaController()
    //{
    //    var appSettingsStub = new Dictionary<string, string?> {
    //        {"Mercadona:BaseUri", "https://tienda.mercadona.es/api/categories/"},
    //        {"Mercadona:DataFilePath", "./Data/MercadonaData.json"},
    //        {"DataUpdateIntervalInHours", "0" }
    //    };

    //    var configuration = new ConfigurationBuilder()
    //        .AddInMemoryCollection(appSettingsStub)
    //        .Build();


    //    var mockFactory = new Mock<IHttpClientFactory>();
    //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    //    mockHttpMessageHandler.Protected()
    //        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
    //        .ReturnsAsync(new HttpResponseMessage
    //        {
    //            StatusCode = HttpStatusCode.OK,
    //            Content = new StringContent("{'name':thecodebuzz,'city':'USA'}"),
    //        });



    //    var client = new HttpClient(mockHttpMessageHandler.Object);
    //    mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);


    //    //return new MercadonaController(configuration, mockFactory.Object);

    //}
}