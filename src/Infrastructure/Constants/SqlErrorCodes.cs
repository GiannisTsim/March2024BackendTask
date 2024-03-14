namespace March2024BackendTask.Infrastructure.Constants;

public static class SqlErrorCodes
{
    public const int ProductDuplicateName = 60004;
    public const int ProductNotFound = 60005;
    public const int ProductCurrencyLost = 60006;
    
    public const int CustomerDuplicateName = 70004;
    public const int CustomerNotFound = 70005;
    public const int CustomerCurrencyLost = 70006;
    
    public const int PurchaseNotFound = 80002;
    public const int PurchaseDuplicateItem = 80003;
    public const int PurchaseAlreadySubmitted = 80004;
    public const int PurchaseNotYetSubmitted = 80005;
    public const int PurchaseAlreadyNotified = 80006;
}
