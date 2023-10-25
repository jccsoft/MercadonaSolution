namespace MercadonaAPI.Models;

#pragma warning disable IDE1006 // Naming Styles

public class CategoryModel
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public int order { get; set; }
    public int layout { get; set; }
    public bool published { get; set; }
    public bool is_extended { get; set; }
    public CategoryModel[]? categories { get; set; }
    public ProductModel[]? products { get; set; }
}
