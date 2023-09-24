using WebAPI.Infrastructure.Handlers.Exceptions;

namespace WebAPI.Infrastructure.Middlewares;

public sealed class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly HttpExceptionHandler _httpExceptionHandler = new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            // TODO: Log exception
            _httpExceptionHandler.HttpResponse = context.Response;
            await _httpExceptionHandler.HandleExceptionAsync(e);
        }
    }
}