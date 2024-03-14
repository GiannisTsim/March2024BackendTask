namespace March2024BackendTask.Core.Purchase.Exceptions;

[Serializable]
public class PurchaseAlreadyNotifiedException : Exception
{
    private const string DefaultMessageTemplate = "Customer '{0}' is already notified for purchase '{1}'.";
    public int CustomerNo { get; }
    public DateTimeOffset PurchaseDtm { get; }

    public PurchaseAlreadyNotifiedException(int customerNo, DateTimeOffset purchaseDtm)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm))
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }

    public PurchaseAlreadyNotifiedException(int customerNo, DateTimeOffset purchaseDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm), inner)
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }
}