using March2024BackendTask.Core.Product.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace March2024BackendTask.WebApi.ExceptionHandlers;

public class ProductExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProductExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception
            is ProductCurrencyLostException
            or ProductDuplicateNameException
            or ProductNotFoundException
           )
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return await _problemDetailsService.TryWriteAsync
            (
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = { Detail = exception.Message },
                    Exception = exception
                }
            );
        }

        return false;
    }
}