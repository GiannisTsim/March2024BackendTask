namespace March2024BackendTask.Core.Product.Exceptions;

[Serializable]
public class ProductDuplicateNameException : Exception
{
    private const string DefaultMessageTemplate = "Product with name '{0}' already exists.";
    public string Name { get; }

    public ProductDuplicateNameException(string name) : base(string.Format(DefaultMessageTemplate, name))
    {
        Name = name;
    }

    public ProductDuplicateNameException(string name, Exception inner) : base
        (string.Format(DefaultMessageTemplate, name), inner)
    {
        Name = name;
    }
}