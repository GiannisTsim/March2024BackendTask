using March2024BackendTask.Core.Customer.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace March2024BackendTask.WebApi.ExceptionHandlers;

public class CustomerExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public CustomerExceptionHandler(IProblemDetailsService problemDetailsService)
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
            is CustomerCurrencyLostException
            or CustomerDuplicateNameException
            or CustomerNotFoundException
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