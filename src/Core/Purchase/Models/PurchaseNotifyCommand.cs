namespace March2024BackendTask.Core.Purchase.Models;

public record PurchaseNotifyCommand(
    int CustomerNo,
    DateTimeOffset PurchaseDtm
);