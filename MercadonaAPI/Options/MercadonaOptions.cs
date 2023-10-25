namespace MercadonaAPI.Options;

public class MercadonaOptions
{
    public const string SettingsName = "MercadonaSettings";
    public const string HttpClientName = "Mercadona";
    public string BaseUrl { get; set; } = string.Empty;
    public string FullDBFilePath { get; set; } = string.Empty;
    public string CategoriesFilePath { get; set; } = string.Empty;
    public string ProductsFilePath { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool ActiveWorkerService { get; set; }

}
