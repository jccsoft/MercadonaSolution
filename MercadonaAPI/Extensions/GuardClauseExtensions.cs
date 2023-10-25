using Ardalis.GuardClauses;
using MercadonaAPI.Shared.Models;
using Microsoft.VisualBasic.FileIO;

namespace MercadonaAPI.Extensions;

public static class GuardClauseExtensions
{
    public static ProductsRequestParams ProductParamsNotReady(this IGuardClause _, ProductsRequestParams parameters)
    {
        if (parameters.ReadyForRequest() == false)
        {
            throw new ArgumentException("Parámetros de consulta no completos", nameof(parameters));
        }
        return parameters;
    }
}
