using March2024BackendTask.Core.Purchase.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace March2024BackendTask.WebApi.ExceptionHandlers;

public class PurchaseExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public PurchaseExceptionHandler(IProblemDetailsService problemDetailsService)
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
            is PurchaseAlreadySubmittedException
            or PurchaseDuplicateItemException
            or PurchaseNotFoundException
            or PurchaseNotYetSubmittedException
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