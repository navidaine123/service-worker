using Common.LogModels;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Context
    {
        private readonly string _connction;
        private readonly ILogger _logger;
        private static bool firstInstance = true;

        public Context(string connction,ILogger logger)
        {
            _connction = connction;
            _logger = logger;
        }
        
        public async Task InitialDataBase()
        {
            var connectionString = new SqlConnectionStringBuilder(_connction);
            var dbName = connectionString.InitialCatalog;
            connectionString.InitialCatalog = "master";

            try
            {
                using (var tmpConn = new SqlConnection(connectionString.ConnectionString))
                {
                    string sqlCreateDBQuery =
                $"if not exists (select * from sys.databases where name ='{dbName}')\n" +
                "begin\n" +
                $"create database {dbName}\n" +
                "end\n";
                    using (var cmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteScalar();
                    }

                }
                using (var tmpConn = new SqlConnection(_connction))
                {
                    var sqlCreateTable =
                "if not exists(select Table_name from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'TblPerson') \n" +
                "begin\n" +
                " create table TblPerson(Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, FirstName char(50), LastName char(50), Age int);  \n" +
                "end\n";
                    using (var cmd = new SqlCommand(sqlCreateTable, tmpConn))
                    {
                        tmpConn.Open();
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteScalar();
                    }

                }
                _logger.Information("Connection has run{@SqlConnectionSucces}", new LogModel(new { }));
            }
            catch (Exception ex)
            {
                _logger.Error("DataBase Connection has failed please check your connection srting and restart app {@SqlConnecTionFailed}",
                    new LogModel(new ErrorLogModel(ex)));
                throw ex;
            }
        }


    }
}
