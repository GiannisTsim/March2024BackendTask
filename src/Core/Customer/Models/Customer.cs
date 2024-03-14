namespace March2024BackendTask.Core.Customer.Models;

public record Customer
{
    public required int CustomerNo { get; init; }
    public required DateTimeOffset UpdatedDtm { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string DiscountPct { get; init; }
}