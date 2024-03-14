namespace March2024BackendTask.Core.Purchase.Models;

public record PurchaseItemAddCommand(
    int CustomerNo,
    DateTimeOffset PurchaseDtm,
    int ProductNo,
    int Quantity
);