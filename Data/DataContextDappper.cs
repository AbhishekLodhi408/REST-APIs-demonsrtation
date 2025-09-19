using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Application1.Data
{
    public class DataContextDappper
    {
        private readonly IConfiguration _configuration;
        public DataContextDappper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
           IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("OurConnectionString"));
            return connection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("OurConnectionString"));
            return connection.QueryFirstOrDefault<T>(sql);
            
        }

        public bool ExecuteSql(string sql) {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("OurConnectionString"));
                return connection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("OurConnectionString"));
            return connection.Execute(sql);
        }

        public bool ExecuteSqlWithParams(string sql, object param)
        {
            IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("OurConnectionString"));
            return connection.Execute(sql, param) > 0;
        }
    }
}
