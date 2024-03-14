namespace March2024BackendTask.Core.Purchase.Exceptions;

[Serializable]
public class PurchaseNotFoundException : Exception
{
    private const string DefaultMessageTemplate = "There is no purchase for customer '{0}' at '{1}'.";
    public int CustomerNo { get; }
    public DateTimeOffset PurchaseDtm { get; }

    public PurchaseNotFoundException(int customerNo, DateTimeOffset purchaseDtm)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm))
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }

    public PurchaseNotFoundException(int customerNo, DateTimeOffset purchaseDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm), inner)
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
    }
}