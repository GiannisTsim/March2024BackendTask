using System.Data;
using Dapper;
using March2024BackendTask.Core.Product.Abstractions;
using March2024BackendTask.Core.Product.Exceptions;
using March2024BackendTask.Core.Product.Models;
using March2024BackendTask.Infrastructure.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace March2024BackendTask.Infrastructure.Repositories;

public sealed class ProductRepository : BaseRepository, IProductRepository
{
    public ProductRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<Product>> GetAsync()
    {
        await using var connection = Connection;
        var products = await connection.QueryAsync<Product>
        (
            """
            SELECT ProductNo, Name, Description, Price, UpdatedDtm
            FROM Product
            WHERE IsObsolete = 0
            """
        );
        return products;
    }

    public async Task<Product?> FindByProductNoAsync(int productNo)
    {
        await using var connection = Connection;
        var product = await connection.QuerySingleOrDefaultAsync<Product>
        (
            """
            SELECT ProductNo, Name, Description, Price, UpdatedDtm
            FROM Product
            WHERE ProductNo = @ProductNo
              AND IsObsolete = 0
            """,
            new { ProductNo = productNo }
        );
        return product;
    }

    public async Task<Product?> FindByNameAsync(string name)
    {
        await using var connection = Connection;
        var product = await connection.QuerySingleOrDefaultAsync<Product>
        (
            """
            SELECT ProductNo, Name, Description, Price, UpdatedDtm
            FROM Product
            WHERE Name = @Name
              AND IsObsolete = 0
            """,
            new { Name = name }
        );
        return product;
    }

    public async Task<int> AddAsync(ProductAddCommand command)
    {
        var p = new DynamicParameters(new { command.Name, command.Description, command.Price });
        p.Add("@ProductNo", DbType.Int32, direction: ParameterDirection.Output);

        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Product_Add_tr",
                p,
                commandType: CommandType.StoredProcedure
            );

            var productNo = p.Get<int>("@ProductNo");
            return productNo;
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductDuplicateName)
        {
            throw new ProductDuplicateNameException(command.Name, ex);
        }
    }

    public async Task ModifyAsync(ProductModifyCommand command)
    {
        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Product_Modify_tr",
                new { command.ProductNo, command.UpdatedDtm, command.Name, command.Description, command.Price },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductNotFound)
        {
            throw new ProductNotFoundException(command.ProductNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductCurrencyLost)
        {
            throw new ProductCurrencyLostException(command.ProductNo, command.UpdatedDtm, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductDuplicateName)
        {
            throw new ProductDuplicateNameException(command.Name, ex);
        }
    }

    public async Task DropAsync(ProductDropCommand command)
    {
        await using var connection = Connection;

        try
        {
            await connection.ExecuteAsync
            (
                "Product_Drop_tr",
                new { command.ProductNo, command.UpdatedDtm },
                commandType: CommandType.StoredProcedure
            );
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductNotFound)
        {
            throw new ProductNotFoundException(command.ProductNo, ex);
        }
        catch (SqlException ex) when (ex.Number == SqlErrorCodes.ProductCurrencyLost)
        {
            throw new ProductCurrencyLostException(command.ProductNo, command.UpdatedDtm, ex);
        }
    }
}