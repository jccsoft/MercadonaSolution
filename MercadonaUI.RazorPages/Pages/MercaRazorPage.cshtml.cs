using MercadonaAPI.Shared.Models;
using MercadonaUI.RazorPages.Helpers;
using MercadonaUI.RazorPages.Models;
using MercadonaUI.RazorPages.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Reflection;

namespace MercadonaUI.RazorPages.Pages;

public class MercaRazorPage : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly ImagesHelper _imagesHelper;


    [BindProperty]
    public ApiResponse<List<ProductModel>>? ProductsResponse { get; set; }

    [BindProperty(SupportsGet = true)]
    public ProductsRequestParams RequestParams { get; set; } = new();


    #region Categories Variables

    [BindProperty(SupportsGet = true)]
    public List<CategoryDto>? Categories { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool LoadedCategories { get; set; }

    [BindProperty]
    public CategoryDto? SelectedCategory { get; set; }

    [BindProperty]
    public List<int> SelectedCategoriesIds { get; set; } = new();

    #endregion


    #region Auxiliar Variables

    [BindProperty]
    public bool RecordsNotFound { get; set; } = false;
    [BindProperty]
    public string ErrorMessage { get; set; } = "";
    [BindProperty]
    public string HeaderMessage { get; set; } = "";

    [BindProperty]
    public int PageMin { get; set; }
    [BindProperty]
    public int PageMax { get; set; }

    #endregion


    public MercaRazorPage(IHttpClientFactory clientFactory, ImagesHelper imagesHelper)
    {
        _httpClient = clientFactory.CreateClient(MercadonaOptions.HttpClientName);
        _imagesHelper = imagesHelper;
    }

    public async Task<IActionResult> OnGet()
    {
        if (LoadedCategories == false)
        {
            // la primera vez descarga las categorías de la api y las guarda en "./Data/Categories.json"
            await GetCategories();
            LoadedCategories = true;
            RequestParams.ItemsXPage = 20;
        }
        else
        {
            // a partir de la segunda vez recupera las categorías del json, por rapidez.
            Categories = JsonHelper<List<CategoryDto>>.ReadFromJson("./Data/Categories.json");
        }

        if (Categories != null && RequestParams.CategoryId > 0)
        {
            SelectedCategoriesIds.Add(RequestParams.CategoryId);
            var parent = Categories.Where(c => c.Id == RequestParams.CategoryId).First();
            while (parent.ParentId > 0)
            {
                SelectedCategoriesIds.Add(parent.ParentId);
                parent = Categories.Where(c => c.Id == parent.ParentId).First();
            }

            SelectedCategory = Categories.Where(c => c.Id == RequestParams.CategoryId).First();

        }

        await GetProducts();

        return Page();
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("./MercaRazorPage", new { RequestParams.Search, RequestParams.CategoryId, RequestParams.CurrentPage, RequestParams.ItemsXPage, LoadedCategories });
    }

    private async Task GetProducts()
    {
        ProductsResponse = null;
        ErrorMessage = "";
        RecordsNotFound = false;

        if (RequestParams.ReadyForRequest())
        {
            string httpRequest = $"products{RequestParams.GetParametersForUrl()}";

            HttpResponseMessage response = await _httpClient!.GetAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                ProductsResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductModel>>>();
                if (ProductsResponse == null || ProductsResponse.Data == null || ProductsResponse.TotalCount == 0)
                {
                    RecordsNotFound = true;
                    return;
                }

                HeaderMessage = $"Mostrando {ProductsResponse.Data.Count} de {ProductsResponse.TotalCount} productos.";
                await _imagesHelper.DownloadProductsImages(ProductsResponse.Data);

                if (ProductsResponse.TotalPages <= 9)
                {
                    PageMin = 1;
                    PageMax = ProductsResponse.TotalPages;
                }
                else
                {
                    PageMin = Math.Max(1, RequestParams.CurrentPage - 3);
                    PageMax = PageMin + 7;
                }

            }
            else
            {
                ErrorMessage = response.ReasonPhrase ?? "Error desconocido";
            }
        }

    }


    private async Task GetCategories()
    {
        HttpResponseMessage response = await _httpClient!.GetAsync("categories");
        if (response.IsSuccessStatusCode)
        {
            var catResponse = response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDto>>>();
            if (catResponse != null && catResponse.Result != null)
            {
                Categories = catResponse.Result.Data;
            }
        }
        else
        {
            Categories = JsonHelper<List<CategoryDto>>.ReadFromJson("./Data/Categories.json");
        }
    }
}
