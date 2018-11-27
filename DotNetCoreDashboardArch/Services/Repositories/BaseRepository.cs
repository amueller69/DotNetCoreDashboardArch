using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DotNetCoreDashboardArch.Services.Repositories
{
    public class BaseRepository
    {
        private string _connString;
        public BaseRepository(string connString)
        {
            _connString = connString;
        }
        protected SqlConnection GetDBContext()
        {
            SqlConnection sqlConn = new SqlConnection(_connString);
            return sqlConn;
        }
    }
}
