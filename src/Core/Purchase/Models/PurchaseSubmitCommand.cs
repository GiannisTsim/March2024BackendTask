namespace March2024BackendTask.Core.Purchase.Models;

public record PurchaseSubmitCommand(
    int CustomerNo,
    DateTimeOffset PurchaseDtm
);