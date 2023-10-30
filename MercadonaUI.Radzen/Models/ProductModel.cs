using MercadonaAPI.Shared.Models;

namespace MercadonaUI.RadzenServer.Models;

public class ProductModel : ProductDto
{
    public string Image => @$"/images/{Path.GetFileName(Thumbnail[..Thumbnail.IndexOf('?')])}";
}
