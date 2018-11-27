using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DotNetCoreDashboardArch.Services
{
    public interface IDBContext : IDisposable
    {
        SqlConnection Connection {get; set; }
    }

    public class DBContext : IDBContext
    {
        public SqlConnection Connection { get; set; }
        public DBContext(string connString)
        {
            Connection = new SqlConnection(connString);
        }
        public void Dispose()
        {
            Connection.Close();
        }
    }
}
