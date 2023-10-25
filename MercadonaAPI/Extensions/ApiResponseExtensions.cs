using MercadonaAPI.Shared.Models;

namespace MercadonaAPI.Extensions;

public static class ApiResponseExtensions
{
    public static ApiResponse<List<ProductDto>> FilterBySearch(this ApiResponse<List<ProductDto>> response, string search)
    {
        if (string.IsNullOrEmpty(search) || response.Data == null) return response;

        var terms = search.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (string term in terms)
        {
            response.Data = response.Data.Where(p => p.Name.ToLower().Contains(term)).ToList();
        }

        return response;
    }

    public static ApiResponse<List<ProductDto>> FilterByCategory(this ApiResponse<List<ProductDto>> response, int categoryId)
    {
        if (categoryId <= 0 || response.Data == null) return response;

        response.Data = response.Data.Where(p => p.CategoryIds.Contains(categoryId)).ToList();

        return response;
    }

    public static ApiResponse<T> UpdateSummary<T>(this ApiResponse<T> response, string fullDbFilePath, int itemsXPage = 0) where T : new()
    {
        if (typeof(T) == typeof(List<CategoryDto>) || typeof(T) == typeof(List<ProductDto>))
        {
            T t = new();
            var p = t.GetType().GetProperty("Count");
            if (p != null)
            {
                response.TotalCount = (int)(p.GetValue(response.Data) ?? 0);
            }
        }
        else
        {
            response.TotalCount = response.Data == null ? 0 : 1;
        }
        if (string.IsNullOrEmpty(fullDbFilePath) == false)
        {
            response.LastReadingFromMercadonaAPI = File.GetLastWriteTime(fullDbFilePath);
        }

        response.TotalPages = (itemsXPage == 0 ? 1 : (int)Math.Ceiling(response.TotalCount / (decimal)itemsXPage));

        return response;
    }

    public static ApiResponse<List<ProductDto>> SkipAndTake(this ApiResponse<List<ProductDto>> response, int itemsXPage, int page)
    {
        if (itemsXPage <= 0 || page <= 0 || response.Data == null) return response;

        response.Data = response.Data.Skip(itemsXPage * (page - 1)).Take(itemsXPage).ToList();

        return response;
    }

}
