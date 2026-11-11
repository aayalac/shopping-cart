using System.Data;
using Microsoft.Data.Sqlite;

namespace ShoppingCart.Api.Shared.Data;

internal sealed class SqlConnectionFactory : ISqlConectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}