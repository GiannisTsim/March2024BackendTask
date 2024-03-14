namespace March2024BackendTask.Core.Product.Exceptions;

[Serializable]
public class ProductCurrencyLostException : Exception
{
    private const string DefaultMessageTemplate = "Product '{0}' data currency is lost.";
    public int ProductNo { get; }
    public DateTimeOffset UpdatedDtm { get; }


    public ProductCurrencyLostException(int productNo, DateTimeOffset updatedDtm)
        : base(string.Format(DefaultMessageTemplate, productNo))
    {
        ProductNo = productNo;
        UpdatedDtm = updatedDtm;
    }

    public ProductCurrencyLostException(int productNo, DateTimeOffset updatedDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, productNo), inner)
    {
        ProductNo = productNo;
        UpdatedDtm = updatedDtm;
    }
}