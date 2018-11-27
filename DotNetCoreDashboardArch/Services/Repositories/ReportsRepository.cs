using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotNetCoreDashboardArch.Models.Reports;


namespace DotNetCoreDashboardArch.Services.Repositories
{
    public interface IReportsRepository
    {
        Task<ReportResults> GetSearchResults(ReportParms searchParms);
        Task<TypeaheadResults> GetTypeAheadResults(TypeaheadParms parms);
    }

    public class ReportsRepository : BaseRepository, IReportsRepository
    {
        public ReportsRepository(string connString) : base(connString) { }

        public async Task<ReportResults> GetSearchResults(ReportParms reportParms)
        {
            string sqlcmd = "Reports_SP";
            var parmsList = new List<SqlParameter>() {
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = reportParms.Name ?? (object) DBNull.Value},
                new SqlParameter("@Type", SqlDbType.NVarChar) {Value = reportParms.Type ?? (object) DBNull.Value},
                new SqlParameter("@PolicyID", SqlDbType.NVarChar) {Value = reportParms.PolicyID ?? (object) DBNull.Value},
                new SqlParameter("@Status", SqlDbType.NVarChar) {Value = reportParms.Status ?? (object) DBNull.Value},
            };
            var results = new List<Dictionary<string, object>>();
            using (var connection = GetDBContext())
            {
                Task open = connection.OpenAsync();
                var command = new SqlCommand(sqlcmd, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parmsList.ToArray());
                await open;
                SqlDataReader reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    int count = reader.FieldCount;
                    while (await reader.ReadAsync())
                    {
                        var rowDict = new Dictionary<string, object>();
                        for (int i = 0; i < count; i++)
                        {
                            object columnVal;
                            string columnName = reader.GetName(i);
                            columnVal = reader.IsDBNull(i) ? "" : await reader.GetFieldValueAsync<object>(i);
                            rowDict.Add(columnName, columnVal);
                        }
                        results.Add(rowDict);
                    }
                }
            }
            return new ReportResults
            {
                ResultsList = results,
                ItemsCount = results.Count
            };
        }

        public async Task<TypeaheadResults> GetTypeAheadResults(TypeaheadParms parms)
        {
            string field = parms.Name;
            string value = parms.Value;
            if (!(new[] { "Name", "Type", "PolicyID" }.Contains(field)))
            {
                throw new ArgumentOutOfRangeException("Typeahead not available for the specified field!");
            }


            string sqlCmd = "searchTypeAhead";
            var results = new List<string>();
            var parmsList = new List<SqlParameter>() {
                new SqlParameter("@name", SqlDbType.NVarChar) {Value = field ?? (object) DBNull.Value},
                new SqlParameter("@value", SqlDbType.NVarChar) {Value = value ?? (object) DBNull.Value}
            };

            using (var connection = GetDBContext())
            {
                Task open = connection.OpenAsync();
                var command = new SqlCommand(sqlCmd, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parmsList.ToArray());
                await open;
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(reader.IsDBNull(0) ? null : reader.GetString(0));
                }
                reader.Dispose();
            }
            return new TypeaheadResults
            {
                Results = results
            };

        }
    }
}
