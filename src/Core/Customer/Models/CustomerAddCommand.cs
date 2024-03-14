namespace March2024BackendTask.Core.Customer.Models;

public record CustomerAddCommand(
    string FirstName,
    string LastName,
    decimal DiscountPct
);