using Ardalis.GuardClauses;
using MercadonaAPI.Clients;
using MercadonaAPI.Extensions;
using MercadonaAPI.Helpers;
using MercadonaAPI.Options;
using MercadonaAPI.Shared.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace MercadonaAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class MercadonaController : ControllerBase
{
    private readonly ILogger<MercadonaController> _logger;
    private readonly MercadonaOptions _settings;
    private readonly MercadonaClient _mercaClient;

    public MercadonaController(ILogger<MercadonaController> logger,
                               IOptions<MercadonaOptions> options,
                               MercadonaClient mercaClient)
    {
        _logger = logger;
        _settings = options.Value;
        _mercaClient = mercaClient;
    }


    [HttpGet("Categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<CategoryDto>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategories()
    {
        ApiResponse<List<CategoryDto>> apiResponse = new();

        try
        {
            apiResponse.Data = JsonHelper<List<CategoryDto>>.ReadFromJson(_settings.CategoriesFilePath);
            Guard.Against.NullOrEmpty(apiResponse.Data, "", "No se han encontrado categorías");

            apiResponse.UpdateSummary(_settings.FullDBFilePath);
        }
        catch (Exception ex)
        {
            apiResponse.ProblemDetail = new(ex, Request);

            _logger.LogCritical(ex, "");

            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        return await Task.FromResult(new ObjectResult(apiResponse));
    }



    [HttpGet("Products")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ProductDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductsRequestParams parameters)
    {
        ApiResponse<List<ProductDto>> apiResponse = new();

        try
        {
            Guard.Against.Null(parameters);
            Guard.Against.ProductParamsNotReady(parameters);

            apiResponse.Data = JsonHelper<List<ProductDto>>.ReadFromJson(_settings.ProductsFilePath);
            Guard.Against.NullOrEmpty(apiResponse.Data, "", $"No se han encontrado productos en {_settings.ProductsFilePath}");

            apiResponse.FilterBySearch(parameters.Search)
                       .FilterByCategory(parameters.CategoryId)
                       .UpdateSummary(_settings.FullDBFilePath, parameters.ItemsXPage)
                       .SkipAndTake(parameters.ItemsXPage, parameters.CurrentPage);
        }
        catch (Exception ex)
        {
            apiResponse.ProblemDetail = new(ex, Request);

            _logger.LogCritical(ex, "");

            Response.StatusCode = (ex.GetType() == typeof(ArgumentException) ?
                                    StatusCodes.Status400BadRequest :
                                    StatusCodes.Status500InternalServerError);
        }

        return await Task.FromResult(new ObjectResult(apiResponse));
    }



    [HttpGet("Products/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetProductById(string productId)
    {
        ApiResponse<ProductDto> apiResponse = new();

        try
        {
            productId = productId.Trim();

            if (productId == "" || productId == "0") productId = "39922"; // testing - borrar

            Guard.Against.NullOrEmpty(productId);
            Guard.Against.OutOfRange(productId.Length, "", 4, 7, $"La longitud de Id debe estar entre 4 y 7 caracteres");
            //List<ProductDto>? products = JsonHelper<List<ProductDto>>.ReadFromJson(_settings.ProductsFilePath);
            //Guard.Against.NullOrEmpty(products, "", "No se han encontrado productos");

            var products = JsonHelper<List<ProductDto>>.ReadFromJson($"{_settings.ProductsFilePath[..^5]}{productId[..2]}.json");
            if (products == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                apiResponse.Data = products.Where(p => p.Id == productId).FirstOrDefault();
                if (apiResponse.Data == null) Response.StatusCode = StatusCodes.Status404NotFound;

                apiResponse.UpdateSummary(_settings.FullDBFilePath);
            }

        }
        catch (Exception ex)
        {
            apiResponse.ProblemDetail = new(ex, Request);

            _logger.LogCritical(ex, "");

            if (Response.StatusCode == StatusCodes.Status200OK)
            {
                Response.StatusCode = (ex.GetType() == typeof(ArgumentException) ?
                                        StatusCodes.Status400BadRequest :
                                        StatusCodes.Status500InternalServerError);
            }
        }

        return await Task.FromResult(new ObjectResult(apiResponse));
    }


    [HttpGet("Products/Landing/{numberOfProducts}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ProductDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLandingProducts(int numberOfProducts)
    {
        ApiResponse<List<ProductDto>> apiResponse = new();

        try
        {
            Guard.Against.NegativeOrZero(numberOfProducts);

            var products = JsonHelper<List<ProductDto>>.ReadFromJson(_settings.ProductsFilePath);
            Guard.Against.NullOrEmpty(products, "", $"No se han encontrado productos en {_settings.ProductsFilePath}");

            apiResponse.Data = new();
            var rng = new Random();
            for (int i = 0; i < numberOfProducts; i++)
            {
                int index = rng.Next(products.Count);
                apiResponse.Data.Add(products.ElementAt(index));
            }

            apiResponse.UpdateSummary(_settings.FullDBFilePath, numberOfProducts);
        }
        catch (Exception ex)
        {
            apiResponse.ProblemDetail = new(ex, Request);

            _logger.LogCritical(ex, "");

            Response.StatusCode = (ex.GetType() == typeof(ArgumentException) ?
                                    StatusCodes.Status400BadRequest :
                                    StatusCodes.Status500InternalServerError);
        }

        return await Task.FromResult(new ObjectResult(apiResponse));
    }

    #region IGNORE

    //[HttpPost("RefreshFullDB")]
    ////[ApiExplorerSettings(IgnoreApi = true)]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> RefreshFullDB(CancellationToken stoppingToken)
    //{
    //    (bool success, string message) = await _mercaClient.GetDataFromAPI(stoppingToken);

    //    if (success)
    //    {
    //        return Ok("Full DB successfully refreshed");
    //    }
    //    else
    //    {
    //        return BadRequest(message);
    //    }
    //}

    //[HttpPost("RefreshCategoriesAndProducts")]
    ////[ApiExplorerSettings(IgnoreApi = true)]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> RefreshCategoriesAndProducts(CancellationToken stoppingToken)
    //{
    //    (bool success, string message) = await _mercaClient.GetCategoriesAndProductsFromData(stoppingToken);

    //    if (success)
    //    {
    //        return Ok("Categories and products successfully refreshed");
    //    }
    //    else
    //    {
    //        return BadRequest(message);
    //    }
    //}
    #endregion
}
