namespace MercadonaAPI.Models;

#pragma warning disable IDE1006 // Naming Styles

public class RootModel
{
    public object? next { get; set; }
    public int count { get; set; }
    public CategoryModel[]? results { get; set; }
    public object? previous { get; set; }
}
