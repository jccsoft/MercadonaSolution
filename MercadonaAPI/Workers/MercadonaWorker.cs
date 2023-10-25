using MercadonaAPI.Clients;
using MercadonaAPI.Options;
using Microsoft.Extensions.Options;

namespace MercadonaAPI.Workers;

public class MercadonaWorker : BackgroundService
{
    private readonly ILogger<MercadonaWorker> _logger;
    private readonly MercadonaOptions _settings;
    private readonly MercadonaClient _client;
    //private readonly TimeOnly _executionTime = new(5, 45);

    public MercadonaWorker(ILogger<MercadonaWorker> logger,
                           IOptions<MercadonaOptions> options,
                           MercadonaClient client)
    {
        _logger = logger;
        _settings = options.Value;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (stoppingToken.IsCancellationRequested == false)
        {

            if (IsDBFileObsolete())
            {
                ShowFullDBFileInfo("anterior");

                await GetAndSaveMercadonaDB(stoppingToken);

                await GetAndSaveCategoriesAndProducts(stoppingToken);

                ShowFullDBFileInfo("nuevo");
            }
            else
            {
                _logger.LogInformation("Worker Service: Ficheros actualizados ({fecha})",
                                       File.GetLastWriteTime(_settings.FullDBFilePath).ToString("dd/MMM HH:mm"));
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            /* Sólo si pasamos a plan básico Azure. El gratis se desconecta a los 20 minutos.
             * 
             * await Task.Delay((int)(NextExecutionTime() - DateTime.Now).TotalMilliseconds, stoppingToken); 
             * await GetAndSaveMercadonaDB(stoppingToken); 
             * 
             */
        }
    }

    private bool IsDBFileObsolete()
    {
        return (File.Exists(_settings.FullDBFilePath) == false
            || File.GetLastWriteTime(_settings.FullDBFilePath) < DateTime.Now.AddDays(-1));
    }

    private void ShowFullDBFileInfo(string text)
    {
        if (File.Exists(_settings.FullDBFilePath))
        {
            _logger.LogInformation("Fichero {texto}: {fichero} => {fecha}",
                                   text,
                                   Path.GetFullPath(_settings.FullDBFilePath),
                                   File.GetLastWriteTime(_settings.FullDBFilePath));
        }
    }

    private async Task GetAndSaveMercadonaDB(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker GetAndSaveMercadonaDB running");

        (bool success, string message) = await _client.GetDataFromAPI(stoppingToken);

        if (success)
        {
            _logger.LogInformation("Success: data saved to {file}", _settings.FullDBFilePath);
        }
        else
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Error: {message}", stoppingToken.IsCancellationRequested ? "Cancelación" : "desconocido");
            }
            else
            {
                _logger.LogError("Error: {message}", message);
            }
        }
    }

    private async Task GetAndSaveCategoriesAndProducts(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker GetAndSaveCategoriesAndProducts running");

        (bool success, string message) = await _client.GetCategoriesAndProductsFromData(stoppingToken);

        if (success)
        {
            _logger.LogInformation("Success: all saved to {file} and {file2}", _settings.CategoriesFilePath, _settings.ProductsFilePath);
        }
        else
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Error: {message}", stoppingToken.IsCancellationRequested ? "Cancelación" : "desconocido");
            }
            else
            {
                _logger.LogError("Error: {message}", message);
            }
        }
    }

    //private DateTime NextExecutionTime()
    //{
    //    DateTime nextExecution = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _executionTime.Hour, _executionTime.Minute, 0);
    //    if (DateTime.Now > nextExecution) nextExecution = nextExecution.AddDays(1);

    //    return nextExecution;
    //}

}
