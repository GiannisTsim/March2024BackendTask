namespace March2024BackendTask.Core.Purchase.Models;

public record PurchaseDetails
{
    public record PurchaseItem
    {
        
        public required int ProductNo {get; init;}
        public required int Quantity {get; init;}
        public required decimal PriceUnit {get; init;}
        public required decimal PriceBatch {get; init;}
    }

    public required int CustomerNo {get; init;}
    public required DateTimeOffset PurchaseDtm {get; init;}
    public required decimal PriceInitial {get; init;}
    public required decimal DiscountPct {get; init;}
    public required decimal Discount {get; init;}
    public required decimal PriceFinal {get; init;}
    public required DateTimeOffset? SubmissionDtm {get; init;}
    public required DateTimeOffset? NotificationDtm {get; init;}
    public required IEnumerable<PurchaseItem> Items {get; init;}
}