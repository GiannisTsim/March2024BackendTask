using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace March2024BackendTask.Infrastructure.Repositories;

public abstract class BaseRepository
{
    private readonly IConfiguration _configuration;
    
    protected BaseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected SqlConnection Connection => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
}