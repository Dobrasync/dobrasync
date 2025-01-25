using Dobrasync.Api.BusinessLogic.Dtos.Error;
using Dobrasync.Api.BusinessLogic.Services.Core.Logger;
using Dobrasync.Api.Shared.Exceptions.Userspace;

namespace Dobrasync.Api.Api.Middleware;

public class ExceptionInterceptorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, ILoggerService logger)
    {
        try
        {
            // Pass request up the chain
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await WriteResponse(ex, httpContext, logger);
        }
    }

    public async Task WriteResponse(Exception ex, HttpContext context, ILoggerService logger)
    {
        ApiErrorDto? errorDto = null;
        if (ex is UserspaceException userspaceException)
        {
            errorDto = GetApiErrorDto(userspaceException);
            logger.LogDebug($"Userspace error during request: {ex.StackTrace}");
        }
        else
        {
            errorDto = new()
            {
                HttpStatusCode = 500,
                Message = "Internal server error",
            };
            logger.LogError($"Internal error during request: {ex.StackTrace}");
        }

        context.Response.StatusCode = errorDto.HttpStatusCode;
        await context.Response.WriteAsJsonAsync(errorDto);
    }
    
    private ApiErrorDto GetApiErrorDto(UserspaceException ex)
    {
        return new ApiErrorDto()
        {
            DateTimeUtc = DateTime.UtcNow,
            Message = ex.UserMessage,
            HttpStatusCode = ex.HttpStatusCode
        };
    }
}