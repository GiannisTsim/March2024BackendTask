namespace March2024BackendTask.Core.Customer.Exceptions;

[Serializable]
public class CustomerCurrencyLostException : Exception
{
    private const string DefaultMessageTemplate = "Customer '{0}' data currency is lost.";
    public int CustomerNo { get; }
    public DateTimeOffset UpdatedDtm { get; }


    public CustomerCurrencyLostException(int customerNo, DateTimeOffset updatedDtm)
        : base(string.Format(DefaultMessageTemplate, customerNo))
    {
        CustomerNo = customerNo;
        UpdatedDtm = updatedDtm;
    }

    public CustomerCurrencyLostException(int customerNo, DateTimeOffset updatedDtm, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo), inner)
    {
        CustomerNo = customerNo;
        UpdatedDtm = updatedDtm;
    }
}