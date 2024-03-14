namespace March2024BackendTask.Core.Product.Models;

public record Product
{
    public required int ProductNo { get; init; }
    public required DateTimeOffset UpdatedDtm { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
}