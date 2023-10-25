using MercadonaAPI.Shared.Models;

namespace MercadonaUI.RazorPages.Models;

public class ProductModel : ProductDto
{
    public string Image => @$"/images/{Path.GetFileName(Thumbnail[..Thumbnail.IndexOf('?')])}";
}
