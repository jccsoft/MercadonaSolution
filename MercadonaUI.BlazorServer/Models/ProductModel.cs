using MercadonaAPI.Shared.Models;

namespace MercadonaUI.BlazorServer.Models;

public class ProductModel : ProductDto
{
    public string Image => @$"/images/{Path.GetFileName(Thumbnail[..Thumbnail.IndexOf('?')])}";
}
