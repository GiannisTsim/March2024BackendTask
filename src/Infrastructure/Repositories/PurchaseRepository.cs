using System.Data;
using Dapper;
using March2024BackendTask.Core.Customer.Exceptions;
using March2024BackendTask.Core.Product.Exceptions;
using March2024BackendTask.Core.Purchase.Abstractions;
using March2024BackendTask.Core.Purchase.Exceptions;
using March2024BackendTask.Core.Purchase.Models;
using March2024BackendTask.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace March2024BackendTask.Infrastructure.Repositories;

public sealed class PurchaseRepository : BaseRepository, IPurchaseRepository
{
    public PurchaseRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<PurchaseSimple>> GetAsync(int? customerNo = null)
    {
        await using var connection = Connection;
        var purchases = await connection.QueryAsync<PurchaseSimple>
        (
            """
            SELECT CustomerNo, PurchaseDtm
            FROM Purchase
            WHERE @CustomerNo IS NULL OR CustomerNo = @CustomerNo
            """,
            new { CustomerNo = customerNo }
        );
        return purchases;
    }

    public async Task<IEnumerable<PurchaseSimple>> GetPendingNotificationsAsync()
    {
        await using var connection = Connection;
        var purchases = await connection.QueryAsync<PurchaseSimple>
        (
            """
            SELECT CustomerNo, PurchaseDtm
            FROM Purchase_V
            WHERE SubmissionDtm IS NOT NULL 
              AND NotificationDtm IS NULL
            """
        );
        return purchases;
    }

    public async Task<PurchaseDetails?> GetDetailsAsync(int customerNo, DateTimeOffset purchaseDtm)
    {
        await using var connection = Connection;
        await using var reader = await connection.QueryMultipleAsync
        (
            """
            SELECT CustomerNo, PurchaseDtm, PriceInitial, DiscountPct, Discount, PriceFinal, SubmissionDtm, NotificationDtm
            FROM Purchase_V
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm;
            SELECT ProductNo, Quantity, PriceUnit, PriceBatch
            FROM PurchaseItem_V
            WHERE CustomerNo = @CustomerNo
              AND PurchaseDtm = @PurchaseDtm;
            """,
            new { CustomerNo = customerNo, PurchaseDtm = purchaseDtm }
        );
        var purchase = await reader.ReadSingleOrDefaultAsync<PurchaseDetails>();
        var purchaseItems = await reader.ReadAsync<PurchaseDetails.PurchaseItem>();
        var purchaseDetails = purchase != null ? purchase with { Items = purchaseItems } : null;
        return purchaseDetails;
    }

    public async Task AddItemAsync(PurchaseItemAddCommand command)
    {
        await using var connection = Connection;
        try
        {
            await connection.ExecuteAsync
            (
                "PurchaseItem_Add_tr",
                new { command.CustomerNo, command.PurchaseDtm, command.ProductNo, command.Quantity },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerNotFound)
        {
            throw new CustomerNotFoundException(command.CustomerNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseAlreadySubmitted)
        {
            throw new PurchaseAlreadySubmittedException(command.CustomerNo, command.PurchaseDtm, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductNotFound)
        {
            throw new ProductNotFoundException(command.ProductNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseDuplicateItem)
        {
            throw new PurchaseDuplicateItemException(command.ProductNo, command.PurchaseDtm, command.ProductNo, ex);
        }
    }

    // TODO: Explore exception handling options when sending multiple statements
    public async Task AddItemBatchAsync(IEnumerable<PurchaseItemAddCommand> commands)
    {
        await using var connection = Connection;
        await connection.ExecuteAsync
        (
            "PurchaseItem_Add_tr",
            commands.Select
                    (
                        command => new { command.CustomerNo, command.PurchaseDtm, command.ProductNo, command.Quantity }
                    )
                    .AsList(),
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task AddSubmitAsync(PurchaseSubmitCommand command)
    {
        await using var connection = Connection;
        try
        {
            await connection.ExecuteAsync
            (
                "PurchaseSubmit_Add_tr",
                new { command.CustomerNo, command.PurchaseDtm },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseNotFound)
        {
            throw new PurchaseNotFoundException(command.CustomerNo, command.PurchaseDtm, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseAlreadySubmitted)
        {
            throw new PurchaseAlreadySubmittedException(command.CustomerNo, command.PurchaseDtm, ex);
        }
    }

    public async Task AddNotificationAsync(PurchaseNotifyCommand command)
    {
        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "PurchaseSubmitNotification_Add_tr",
                new { command.CustomerNo, command.PurchaseDtm },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseNotYetSubmitted)
        {
            throw new PurchaseNotYetSubmittedException(command.CustomerNo, command.PurchaseDtm, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.PurchaseAlreadyNotified)
        {
            throw new PurchaseAlreadyNotifiedException(command.CustomerNo, command.PurchaseDtm, ex);
        }
    }
}