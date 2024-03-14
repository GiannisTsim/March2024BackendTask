namespace March2024BackendTask.Core.Purchase.Exceptions;

[Serializable]
public class PurchaseDuplicateItemException : Exception
{
    private const string DefaultMessageTemplate = "Purchase for customer '{0}' at '{1}' already contains product {2}.";
    public int CustomerNo { get; }
    public DateTimeOffset PurchaseDtm { get; }
    public int ProductNo { get; }

    public PurchaseDuplicateItemException(int customerNo, DateTimeOffset purchaseDtm, int productNo)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm, productNo))
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
        ProductNo = productNo;
    }

    public PurchaseDuplicateItemException(int customerNo, DateTimeOffset purchaseDtm, int productNo, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo, purchaseDtm, productNo), inner)
    {
        CustomerNo = customerNo;
        PurchaseDtm = purchaseDtm;
        ProductNo = productNo;
    }
}