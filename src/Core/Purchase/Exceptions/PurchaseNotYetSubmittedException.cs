namespace March2024BackendTask.Core.Purchase.Exceptions;

[Serializable]
public class PurchaseNotYetSubmittedException : Exception
{
    private const string DefaultMessageTemplate = "Purchase for customer '{0}' at '{1}' is not yet submitted.";
    public int CustomerNo { get; }
    public DateTimeOffset PurchaseDtm { get; }

    public PurchaseNotYetSubmittedException(int customerNo, DateTimeOffset purchaseDtm)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm))
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }

    public PurchaseNotYetSubmittedException(int customerNo, DateTimeOffset purchaseDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm), inner)
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }
}