using MercadonaAPI.Shared.Models;

namespace MercadonaUI.BlazorServer.Models;

public class ProductModel : ProductDto
{
    public string Image
    {
        get
        {
            if (Thumbnail.IndexOf('?') < 0) return "";
            return @$"/images/{Path.GetFileName(Thumbnail[..Thumbnail.IndexOf('?')])}";
        }
    }
}
