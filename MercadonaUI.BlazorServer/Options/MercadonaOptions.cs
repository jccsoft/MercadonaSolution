namespace MercadonaUI.BlazorServer.Options;

public class MercadonaOptions
{
    public const string SettingsName = "MercadonaSettings";
    public const string HttpClientName = "Mercadona";
    public string AzureBaseUriString { get; set; } = string.Empty;
    public string LocalBaseUriString { get; set; } = string.Empty;
}
