namespace MercadonaAPI.Shared.Models;

public class ApiResponse<T>
{
    public int TotalCount { get; set; }

    public T? Data { get; set; }

    public int TotalPages { get; set; }
    public DateTime? LastReadingFromMercadonaAPI { get; set; }

    public ApiProblemDetails? ProblemDetail { get; set; }

}
