namespace March2024BackendTask.Core.Customer.Exceptions;

[Serializable]
public class CustomerNotFoundException: Exception
{
    private const string DefaultMessageTemplate = "Customer '{0}' does not exist.";
    public int CustomerNo { get; }

    public CustomerNotFoundException(int customerNo) : base(string.Format(DefaultMessageTemplate, customerNo))
    {
        CustomerNo = customerNo;
    }

    public CustomerNotFoundException(int customerNo, Exception inner)
        : base(string.Format(DefaultMessageTemplate, customerNo), inner)
    {
        CustomerNo = customerNo;
    }
}