using March2024BackendTask.Core.Product.Models;

namespace March2024BackendTask.Core.Product.Abstractions;

public interface IProductRepository
{
    Task<IEnumerable<Models.Product>> GetAsync();
    Task<Models.Product?> FindByProductNoAsync(int productNo);
    Task<Models.Product?> FindByNameAsync(string name);
    Task<int> AddAsync(ProductAddCommand command);
    Task ModifyAsync(ProductModifyCommand command);
    Task DropAsync(ProductDropCommand command);
}