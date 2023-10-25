using Castle.Core.Logging;
using MercadonaAPI.Clients;
using MercadonaAPI.Mapping;
using MercadonaAPI.Tests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Serilog;
using Serilog.Events;
using System.IO;
using System.Reflection;
using MO = MercadonaAPI.Options.MercadonaOptions;

namespace MercadonaAPI.Tests;

public class MercadonaClientTests
{
    private ILogger<MercadonaClient>? _logger;// = new NullLogger<MercadonaClient>();
    private IOptions<MO>? _options;
    private IHttpClientFactory? _factory;


    private readonly string _outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");

    public MercadonaClientTests()
    {
        SetupMockILogger();
        SetupMockIHttpClientFactory();
        //SetupMockIMapper();
    }


    [Fact]
    public async Task GetDataFromAPI_StandardTest()
    {
        // Arrange
        string workFolder = SetupMockIOptions("Test1");
        var client = new MercadonaClient(_logger!, _options!, _factory!);

        FileHelper.DeleteAllFilesInFolder(workFolder);

        // Act
        (bool success, string message) = await client.GetDataFromAPI(new CancellationToken());

        // Assert
        Assert.True(success, message);
        Assert.True(File.Exists(_options!.Value.FullDBFilePath));
        Assert.False(File.Exists(_options!.Value.CategoriesFilePath));
        Assert.False(File.Exists(_options!.Value.ProductsFilePath));
        Assert.NotNull(client.Model);
        Assert.NotNull(client.Model.results);
        Assert.Single(client.Model.results);

    }


    [Fact]
    public async Task GetCategoriesAndProductsFromData_StandardTest()
    {
        // Arrange
        string sampleFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SampleApiData/9Cats35Prods.json");
        string workFolder = SetupMockIOptions("Test2", sampleFilePath);
        var client = new MercadonaClient(_logger!, _options!, _factory!);
        FileHelper.DeleteAllFilesInFolder(workFolder);

        // Act
        (bool success, string message) = await client.GetCategoriesAndProductsFromData(new CancellationToken());

        // Assert
        Assert.True(success, message);
        Assert.True(File.Exists(_options!.Value.CategoriesFilePath));
        Assert.True(File.Exists(_options!.Value.ProductsFilePath));
        Assert.NotNull(client.Categories.Data);
        Assert.NotNull(client.Products.Data);
        Assert.Equal(9, client.Categories.Data.Count);
        Assert.Equal(35, client.Products.Data.Count);
    }


    #region SETUP MOCKS

    private string SetupMockIOptions(string outputSubFolder, string fullDBFileName = "")
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets("5a014dc9-2a17-4ffe-81fa-ecb5d3f385b5") // UserSecretsId en Project properties
            .Build();

        MO options = new();
        config.GetSection(MO.SettingsName).Bind(options);

        string workFolder = _outputFolder + "\\" + outputSubFolder;
        Directory.CreateDirectory(workFolder);
        options.FullDBFilePath = fullDBFileName.Length > 0 ? fullDBFileName : Path.Combine(workFolder, "testFullDB.json");
        options.CategoriesFilePath = Path.Combine(workFolder, "testCategories.json");
        options.ProductsFilePath = Path.Combine(workFolder, "testProducts.json");

        _options = Substitute.For<IOptions<MO>>();
        _options.Value.Returns(options);

        return workFolder;
    }


    private void SetupMockILogger()
    {
        var builder = WebApplication.CreateBuilder();

        var baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent;
        string logPath = $@"{Path.Combine(baseDirectory!.FullName, "Logs")}\LogTest_.txt";

        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.With(new DateTimeEnricher())
                    .WriteTo.File(path: logPath,
                                  rollingInterval: RollingInterval.Day,
                                  outputTemplate: "{SpainTimeStamp} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateBootstrapLogger();

        builder.Host.UseSerilog();

        _logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<MercadonaClient>>();
    }

    private void SetupMockIHttpClientFactory()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(MO.HttpClientName, (serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MO>>().Value;
            client.BaseAddress = new Uri(_options!.Value.BaseUrl);
            foreach (var kvpair in _options!.Value.Headers)
            {
                if (string.IsNullOrWhiteSpace(kvpair.Value) == false)
                {
                    client.DefaultRequestHeaders.Add(kvpair.Key, kvpair.Value);
                }
            }
        });
        //services.ConfigureServices();
        var provider = services.BuildServiceProvider();
        _factory = provider.GetRequiredService<IHttpClientFactory>();
    }

    //private void SetupMockIMapper()
    //{
    //    var mc = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMappingProfiles()));
    //    _mapper = mc.CreateMapper();
    //    //_mapper = Substitute.For<IMapper>();
    //}
    #endregion
}
