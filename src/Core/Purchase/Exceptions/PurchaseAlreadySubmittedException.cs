namespace March2024BackendTask.Core.Purchase.Exceptions;

[Serializable]
public class PurchaseAlreadySubmittedException : Exception
{
    private const string DefaultMessageTemplate = "Purchase for customer '{0}' at '{1}' is already submitted.";
    public int CustomerNo { get; }
    public DateTimeOffset PurchaseDtm { get; }

    public PurchaseAlreadySubmittedException(int customerNo, DateTimeOffset purchaseDtm)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm))
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }

    public PurchaseAlreadySubmittedException(int customerNo, DateTimeOffset purchaseDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm), inner)
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }
}