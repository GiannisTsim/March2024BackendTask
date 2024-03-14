namespace March2024BackendTask.Core.Product.Models;

public record ProductDropCommand(
    int ProductNo,
    DateTimeOffset UpdatedDtm
);