namespace MercadonaAPI.Tests.Helpers;

public static class FileHelper
{
    public static void DeleteAllFilesInFolder(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }
}
