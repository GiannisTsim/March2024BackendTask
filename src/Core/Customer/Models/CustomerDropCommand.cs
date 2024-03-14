namespace March2024BackendTask.Core.Customer.Models;

public record CustomerDropCommand(
    int CustomerNo,
    DateTimeOffset UpdatedDtm
);