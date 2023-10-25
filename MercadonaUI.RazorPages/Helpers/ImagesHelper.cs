using MercadonaUI.RazorPages.Models;
using MercadonaAPI.Shared.Models;

namespace MercadonaUI.RazorPages.Helpers;

public class ImagesHelper
{
    private readonly HttpClient _httpClient;

    public ImagesHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DownloadProductsImages(List<ProductModel> products)
    {
        foreach (var product in products)
        {
            await DownloadAndSave(product.Thumbnail, "./wwwroot/images", Path.GetFileName(product.Image));
        }
    }

    public async Task DownloadAndSave(string sourceFile, string destinationFolder, string destinationFileName)
    {
        //if (Uri.TryCreate(sourceFile, UriKind.Absolute, out _) == false) return "";
        //if (sourceFile.Contains('?')) sourceFile = sourceFile[..sourceFile.IndexOf('?')];
        //string destinationFileName = Path.GetFileName(sourceFile);

        string destinationFile = Path.Combine(destinationFolder, destinationFileName);

        if (File.Exists(destinationFile) == false)
        {
            Stream fileStream = await GetFileStream(sourceFile);

            if (fileStream != Stream.Null)
            {
                await SaveStream(fileStream, destinationFile);
            }
        }

    }

    private async Task<Stream> GetFileStream(string fileUrl)
    {
        try
        {
            Stream fileStream;
            HttpResponseMessage? response = await _httpClient.GetAsync(fileUrl);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    fileStream = await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    return Stream.Null;
                }
            }
            else
            {
                return Stream.Null;
            }

            return fileStream;
        }
        catch (HttpRequestException)
        {
            return Stream.Null;
        }
        catch (Exception)
        {
            return Stream.Null;
        }
    }

    private static async Task SaveStream(Stream fileStream, string destinationFile)
    {
        //if (!Directory.Exists(destinationFolder))
        //    Directory.CreateDirectory(destinationFolder);

        try
        {

            using FileStream outputFileStream = new(destinationFile, FileMode.CreateNew);

            await fileStream.CopyToAsync(outputFileStream);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

