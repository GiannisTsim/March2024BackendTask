namespace March2024BackendTask.Core.Product.Exceptions;

[Serializable]
public class ProductNotFoundException: Exception
{
    private const string DefaultMessageTemplate = "Product '{0}' does not exist.";
    public int ProductNo { get; }

    public ProductNotFoundException(int productNo) : base(string.Format(DefaultMessageTemplate, productNo))
    {
        ProductNo = productNo;
    }

    public ProductNotFoundException(int productNo, Exception inner)
        : base(string.Format(DefaultMessageTemplate, productNo), inner)
    {
        ProductNo = productNo;
    }
}