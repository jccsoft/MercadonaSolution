using Ardalis.GuardClauses;
using MercadonaAPI.Helpers;
using MercadonaAPI.Mapping;
using MercadonaAPI.Models;
using MercadonaAPI.Options;
using MercadonaAPI.Shared.Models;
using Microsoft.Extensions.Options;

namespace MercadonaAPI.Clients;

public class MercadonaClient
{
    private readonly MercadonaOptions _settings;
    private readonly HttpClient _client;
    private readonly ILogger<MercadonaClient> _logger;

    public RootModel? Model { get; private set; }
    public ApiResponse<List<CategoryDto>> Categories { get; private set; } = new();
    public ApiResponse<List<ProductDto>> Products { get; private set; } = new();


    public MercadonaClient(ILogger<MercadonaClient> logger,
                           IOptions<MercadonaOptions> options,
                           IHttpClientFactory factory)
    {
        _settings = options.Value;
        _client = factory.CreateClient(MercadonaOptions.HttpClientName);
        _logger = logger;
    }


    #region DATA FROM API

    public async Task<(bool success, string message)> GetDataFromAPI(CancellationToken stoppingToken)
    {
        (bool success, string message) output = (false, "");

        if (stoppingToken.IsCancellationRequested) return output;

        try
        {
            HttpResponseMessage response = await _client.GetAsync("", stoppingToken);
            if (response.IsSuccessStatusCode == false) throw new Exception(response.ReasonPhrase);

            Model = await response.Content.ReadFromJsonAsync<RootModel>(cancellationToken: stoppingToken);

            Guard.Against.Null(Model);
            Guard.Against.Null(Model.results);

            foreach (var category in Model.results)
            {
                await GetInnerDataFromAPI(category, stoppingToken);
            }

            SaveDataFromAPI();

            output.success = true;

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "");
            output.message = ex.Message;
        }

        return output;
    }


    private async Task GetInnerDataFromAPI(CategoryModel inputCategory, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested) return;

        bool productionMode = _settings.BaseUrl.ToLower().Contains("mercadona");

        try
        {
            /* La primera request (api/categories) sólo devuelve las categorías que tienen otras sub-categorías.
             * Hay que hacer una request individual a las últimas hijas de este árbol para obtener el detalle
             * de las categorías finales (sin sub-categorías) y sus productos.
             * 
             * Ejemplo:
             *      results: [
             *          {
             *          "id": 12,
             *          "name": "Aceite, especias y salsas",
             *          "categories": [
             *              {
             *              "id": 112,
             *              "name": "Aceite, vinagre y sal",
             *              ...
             * => Ésta última 112 ni siquiera tiene el campo "categories"
             * => Si hacemos un request (/api/categories/112) obtendremos sus categorías y los productos dentro de éstas:
             *      {
             *      "id": 112,
             *      "name": "Aceite, vinagre y sal",
             *      "categories": [
             *          {
             *          "id": 420,
             *          "name": "Aceite de oliva",
             *          "products": [
             *              {
             *              "id": "4240",
             *              "slug": "aceite-oliva-04o-hacendado-botella",
             *              ...
             */
            if (inputCategory.categories == null && inputCategory.products == null)
            {
                if (productionMode)
                {
                    /* Si consultamos la API de mercadona, tenemos que hacer pausas 
                     * para evitar el riesgo de que nos rechace con 429-Too many requests
                     */
                    await Task.Delay(2000, stoppingToken);
                }
                HttpResponseMessage response = await _client.GetAsync($"{inputCategory.id}/", stoppingToken);

                if (response.IsSuccessStatusCode)
                {

                    CategoryModel? innerCategory = await response.Content.ReadFromJsonAsync<CategoryModel>(cancellationToken: stoppingToken);
                    inputCategory.categories = innerCategory?.categories;
                    inputCategory.products = innerCategory?.products;
                }
                else
                {
                    if (productionMode) throw new Exception(response.ReasonPhrase);
                }
            }

            if (inputCategory.categories == null) return;

            for (int i = 0; i < inputCategory.categories.Length; i++)
            {
                await GetInnerDataFromAPI(inputCategory.categories[i], stoppingToken);
            }

        }
        catch (Exception)
        {
            throw;
        }
    }


    private void SaveDataFromAPI()
    {
        try
        {
            JsonHelper<RootModel>.WriteToJson(Model, _settings.FullDBFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "");
            throw;
        }
    }

    private void LoadDataFromFile()
    {
        try
        {
            Model = JsonHelper<RootModel>.ReadFromJson(_settings.FullDBFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "");
            throw;
        }
    }

    #endregion



    #region CATEGORIES AND PRODUCTS

    public async Task<(bool success, string message)> GetCategoriesAndProductsFromData(CancellationToken stoppingToken)
    {
        (bool success, string message) output = (false, "");

        if (stoppingToken.IsCancellationRequested) return output;

        try
        {

            if (Model == null) LoadDataFromFile();
            Guard.Against.Null(Model);
            Guard.Against.Null(Model.results);


            Categories.Data = new();
            Products.Data = new();

            foreach (var category in Model.results)
            {
                await GetInnerCategoriesAndProducts(category, new List<int>(), 0, stoppingToken);
            }

            SaveCategoriesAndProducts();

            output.success = true;

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "");
            output.message = ex.Message;
        }

        return output;
    }


    private async Task GetInnerCategoriesAndProducts(CategoryModel parentCategory, List<int> previousCategories, int parentId, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested) return;

        try
        {
            List<int> currentCategories = previousCategories.ToList();
            currentCategories.Add(parentCategory.id);

            Categories.Data!.Add(new CategoryDto(parentCategory.id, parentCategory.name, parentId));

            if (parentCategory.categories != null)
            {
                for (int i = 0; i < parentCategory.categories.Length; i++)
                {
                    await GetInnerCategoriesAndProducts(parentCategory.categories[i], currentCategories, parentCategory.id, stoppingToken);
                }
            }

            if (parentCategory.products == null) return;

            for (int i = 0; i < parentCategory.products.Length; i++)
            {
                if (Products.Data!.Exists(p => p.Id.ToString() == parentCategory.products[i].id) == false) // evitar guardar productos repetidos
                {
                    var product = parentCategory.products[i].ToProductDto();
                    product.CategoryIds = currentCategories;
                    Products.Data!.Add(product);
                }
            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    private void SaveCategoriesAndProducts()
    {
        JsonHelper<List<CategoryDto>>.WriteToJson(Categories.Data, _settings.CategoriesFilePath);

        if (Products.Data != null)
        {
            // Full DB
            JsonHelper<List<ProductDto>>.WriteToJson(Products.Data, _settings.ProductsFilePath);

            // Partials for GetById
            string fileBase = _settings.ProductsFilePath[..^5];
            for (int i = 0; i < 100; i++)
            {
                string startId = string.Format("{0:00}", i);
                var partialProducts = Products.Data!.Where(p => p.Id.StartsWith(startId)).ToList();
                if (partialProducts.Count > 0)
                {
                    JsonHelper<List<ProductDto>>.WriteToJson(partialProducts, $"{fileBase}{startId}.json");
                }
            }
        }
    }

    #endregion

}
