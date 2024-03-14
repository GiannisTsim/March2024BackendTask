using System.Data;
using Dapper;
using March2024BackendTask.Core.Customer.Abstractions;
using March2024BackendTask.Core.Customer.Exceptions;
using March2024BackendTask.Core.Customer.Models;
using March2024BackendTask.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace March2024BackendTask.Infrastructure.Repositories;

public sealed class CustomerRepository : BaseRepository, ICustomerRepository
{
    public CustomerRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Customer>> GetAsync()
    {
        await using var connection = Connection;
        var customers = await connection.QueryAsync<Customer>
        (
            """
            SELECT CustomerNo, FirstName, LastName, DiscountPct, UpdatedDtm
            FROM Customer
            WHERE IsObsolete = 0
            """
        );
        return customers;
    }

    public async Task<Customer?> FindByCustomerNoAsync(int customerNo)
    {
        await using var connection = Connection;
        var customer = await connection.QuerySingleOrDefaultAsync<Customer>
        (
            """
            SELECT CustomerNo, FirstName, LastName, DiscountPct, UpdatedDtm
            FROM Customer
            WHERE CustomerNo = @CustomerNo
              AND IsObsolete = 0
            """,
            new { CustomerNo = customerNo }
        );
        return customer;
    }

    public async Task<Customer?> FindByFullNameAsync(string firstName, string lastName)
    {
        await using var connection = Connection;
        var customer = await connection.QuerySingleOrDefaultAsync<Customer>
        (
            """
            SELECT CustomerNo, FirstName, LastName, DiscountPct, UpdatedDtm
            FROM Customer
            WHERE FirstName = @FirstName
              AND LastName = @LastName
              AND IsObsolete = 0
            """,
            new { FirstName = firstName, LastName = lastName }
        );
        return customer;
    }

    public async Task<int> AddAsync(CustomerAddCommand command)
    {
        var p = new DynamicParameters(new { command.FirstName, command.LastName, command.DiscountPct });
        p.Add("@CustomerNo", DbType.Int32, direction: ParameterDirection.Output);

        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Customer_Add_tr",
                p,
                commandType: CommandType.StoredProcedure
            );

            var customerNo = p.Get<int>("@CustomerNo");
            return customerNo;
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerDuplicateName)
        {
            throw new CustomerDuplicateNameException(command.FirstName, command.LastName, ex);
        }
    }

    public async Task ModifyAsync(CustomerModifyCommand command)
    {
        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Customer_Modify_tr",
                new
                {
                    command.CustomerNo, command.UpdatedDtm, command.FirstName, command.LastName, command.DiscountPct
                },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerNotFound)
        {
            throw new CustomerNotFoundException(command.CustomerNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerCurrencyLost)
        {
            throw new CustomerCurrencyLostException(command.CustomerNo, command.UpdatedDtm, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerDuplicateName)
        {
            throw new CustomerDuplicateNameException(command.FirstName, command.LastName, ex);
        }
    }

    public async Task DropAsync(CustomerDropCommand command)
    {
        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Customer_Drop_tr",
                new { command.CustomerNo, command.UpdatedDtm },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerNotFound)
        {
            throw new CustomerNotFoundException(command.CustomerNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.CustomerCurrencyLost)
        {
            throw new CustomerCurrencyLostException(command.CustomerNo, command.UpdatedDtm, ex);
        }
    }
}