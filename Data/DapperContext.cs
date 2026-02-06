using Microsoft.Data.SqlClient;
using System.Data;

namespace SimpleCRM.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionLiveString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UATNtsdb")
                ?? throw new InvalidOperationException("Connection string 'UATNtsdb' not found.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public IDbConnection CreateLiveConnection()
            => new SqlConnection(_connectionLiveString);
    }
}
