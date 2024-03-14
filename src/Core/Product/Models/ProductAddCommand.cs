namespace March2024BackendTask.Core.Product.Models;

public record ProductAddCommand(
    string Name,
    string Description,
    decimal Price
);