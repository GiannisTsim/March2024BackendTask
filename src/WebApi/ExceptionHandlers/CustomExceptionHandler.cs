using March2024BackendTask.Core.Customer.Exceptions;
using March2024BackendTask.Core.Product.Exceptions;
using March2024BackendTask.Core.Purchase.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace March2024BackendTask.WebApi.ExceptionHandlers;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    private static bool IsCustomerException(Exception ex) => ex is CustomerCurrencyLostException
        or CustomerDuplicateNameException or CustomerNotFoundException;

    private static bool IsProductException(Exception ex) => ex is ProductCurrencyLostException
        or ProductDuplicateNameException or ProductNotFoundException;

    private static bool IsPurchaseException(Exception ex) => ex is PurchaseAlreadySubmittedException
        or PurchaseDuplicateItemException or PurchaseNotFoundException or PurchaseNotYetSubmittedException;

    public CustomExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (IsCustomerException(exception) || IsProductException(exception) || IsPurchaseException(exception))
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