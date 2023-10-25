using System.ComponentModel.DataAnnotations;

namespace MercadonaAPI.Shared.Models;

public class ProductsRequestParams
{
    public string Search { get; set; } = string.Empty;

    [Range(0, Int32.MaxValue)]
    public int CategoryId { get; set; } = 0;

    [Range(1, 100)]
    public int ItemsXPage { get; set; } = 10;

    [Range(1, Int32.MaxValue)]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Devuelve un string con los parámetros necesarios para completar la requestUri en un Get.
    /// </summary>
    /// <returns></returns>
    public string GetParametersForUrl()
    {
        string httpRequest = $"?{nameof(ItemsXPage)}={ItemsXPage}";
        httpRequest += $"&{nameof(CurrentPage)}={CurrentPage}";
        if (Search.Trim().Length > 0) httpRequest += $"&{nameof(Search)}={Search.Trim()}";
        if (CategoryId > 0) httpRequest += $"&{nameof(CategoryId)}={CategoryId}";

        return httpRequest;
    }

    /// <summary>
    /// Devuelve true si los parámetros necesarios para la consulta se han rellenado (Search y/o CategoryId)
    /// </summary>
    public bool ReadyForRequest()
    {
        return string.IsNullOrWhiteSpace(Search) == false || CategoryId > 0;
    }

}
