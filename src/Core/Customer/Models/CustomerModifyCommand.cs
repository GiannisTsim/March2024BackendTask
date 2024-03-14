namespace March2024BackendTask.Core.Customer.Models;

public record CustomerModifyCommand(
    int CustomerNo,
    DateTimeOffset UpdatedDtm,
    string FirstName,
    string LastName,
    decimal DiscountPct
);