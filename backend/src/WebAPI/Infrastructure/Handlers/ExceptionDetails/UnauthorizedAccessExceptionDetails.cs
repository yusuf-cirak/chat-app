using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Infrastructure.Handlers.ExceptionDetails;

public sealed class UnauthorizedAccessExceptionDetails : ProblemDetails
{
    public UnauthorizedAccessExceptionDetails(string detail)
    {
        Status = StatusCodes.Status401Unauthorized;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5";
        Detail = detail;
        Title = "Forbidden Access";
    }
}