using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;


namespace MercadonaUI.RazorPages.Helpers;

//public class JsonFileUtils<T> where T : new()
public static class JsonHelper<T> where T : new()
{

    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin)
    };


    public static T? ReadFromJson(string filePath)
    {
        using FileStream json = File.OpenRead(filePath);
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public static void WriteToJson(T? obj, string fileName)
    {
        if (obj != null)
        {
            string? directory = Path.GetDirectoryName(fileName);
            if (directory != null && Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            if (Directory.Exists(directory))
            {
                using var fileStream = File.Create(fileName);
                using var utf8JsonWriter = new Utf8JsonWriter(fileStream);
                JsonSerializer.Serialize(utf8JsonWriter, obj, _options);
            }
        }
    }
}
