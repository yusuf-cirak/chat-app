using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Infrastructure.Handlers.ExceptionDetails;

public sealed class ForbiddenAccessExceptionDetails : ProblemDetails
{
    public ForbiddenAccessExceptionDetails(string detail)
    {
        Status = StatusCodes.Status403Forbidden;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
        Detail = detail;
        Title = "Forbidden Access";
    }
}