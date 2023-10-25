namespace MercadonaAPI.Models;

#pragma warning disable IDE1006 // Naming Styles

public class ProductModel
{
    public string? id { get; set; }
    public string? slug { get; set; }
    public int? limit { get; set; }
    public BadgesModel? badges { get; set; }
    public string? packaging { get; set; }
    public bool published { get; set; }
    public string? share_url { get; set; }
    public string? thumbnail { get; set; }
    public ProductCategoryModel[]? categories { get; set; }
    public string? display_name { get; set; }
    public PriceInstructionsModel? price_instructions { get; set; }
    public object[]? unavailable_weekdays { get; set; }
}
