namespace March2024BackendTask.Core.Purchase.Models;

public record PurchaseSimple
{
    public required int CustomerNo {get; init;}
    public required DateTimeOffset PurchaseDtm {get; init;}
}