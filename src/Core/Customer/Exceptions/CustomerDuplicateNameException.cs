namespace March2024BackendTask.Core.Customer.Exceptions;

[Serializable]
public class CustomerDuplicateNameException : Exception
{
    private const string DefaultMessageTemplate = "Customer with first name '{0}' and last name '{1}' already exists.";
    public string FirstName { get; }
    public string LastName { get; }

    public CustomerDuplicateNameException(string firstName, string lastName) : base
        (string.Format(DefaultMessageTemplate, firstName, lastName))
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public CustomerDuplicateNameException(string firstName, string lastName, Exception inner)
        : base(string.Format(DefaultMessageTemplate, firstName, lastName), inner)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}