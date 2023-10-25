namespace MercadonaAPI.Models;

#pragma warning disable IDE1006 // Naming Styles

public class ProductCategoryModel
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public int level { get; set; }
    public int order { get; set; }
}
