using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace FastLane.Dapper
{
    public class DatabaseContext
    {
        private string ConnectionString;

        public DatabaseContext()
        {
            // Chuỗi kết nối  đến PostgresSQL
            ConnectionString = "Host=localhost;Port=5433;Database=FastLane;Username=postgres;Password=12345678";
        }

        //only for postgres
        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        // only for sql server
        //public SqlConnection GetConnection()
        //{
        //    return new SqlConnection(ConnectionString);
        //}
    }
}
