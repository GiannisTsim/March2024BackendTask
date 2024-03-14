using March2024BackendTask.Core.Customer.Models;

namespace March2024BackendTask.Core.Customer.Abstractions;

public interface ICustomerRepository
{
    Task<IEnumerable<Models.Customer>> GetAsync();
    Task<Models.Customer?> FindByCustomerNoAsync(int customerNo);
    Task<Models.Customer?> FindByFullNameAsync(string firstName, string lastName);
    Task<int> AddAsync(CustomerAddCommand command);
    Task ModifyAsync(CustomerModifyCommand command);
    Task DropAsync(CustomerDropCommand command);
}