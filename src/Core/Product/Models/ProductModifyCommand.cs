namespace March2024BackendTask.Core.Product.Models;

public record ProductModifyCommand(
    int ProductNo,
    DateTimeOffset UpdatedDtm,
    string Name,
    string Description,
    decimal Price
);