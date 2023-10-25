using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MercadonaAPI.Shared.Models;

public class ApiProblemDetails : ProblemDetails
{
    public ApiProblemDetails(Exception ex, HttpRequest httpRequest)
    {
        int indexDivider = ex.Message.IndexOf("Path:");
        if (indexDivider > 0)
        {
            Title = ex.Message[..indexDivider];
            Detail = ex.Message[indexDivider..];
        }
        else
        {
            Title = ex.Message;
        }

        Instance = httpRequest.GetDisplayUrl();
    }
}