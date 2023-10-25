namespace MercadonaAPI.Shared.Models;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string ShareUrl { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public string UnitPriceString { get; set; } = string.Empty;
    public decimal ReferencePrice { get; set; }
    public string ReferencePriceString { get; set; } = string.Empty;
    public string Packaging { get; set; } = string.Empty;

    public List<int> CategoryIds { get; set; } = new();
}
