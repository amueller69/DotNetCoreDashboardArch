using System;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using DotNetCoreDashboardArch.Models.Home;

namespace DotNetCoreDashboardArch.Services.Repositories
{
    public interface IHomeRepository
    {
        Task<SummaryStats> GetSummaryAsync();
    }


    public class HomeRepository : BaseRepository, IHomeRepository
    {
        public HomeRepository(string connString) : base(connString)
        {
        }


        public async Task<SummaryStats> GetSummaryAsync()
        {
            string sqlcmd = "EXEC [DBO].[SummarySet1]; EXEC [DBO].[SummarySet2];";
            var results = new Dictionary<string, Dictionary<string, int>>();
            string resultKey;
            using (var connection = GetDBContext())
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sqlcmd.ToString(), connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();
                while (reader.HasRows)
                {
                    resultKey = reader.GetName(0);
                    results.Add(resultKey, new Dictionary<string, int>());
                    while (await reader.ReadAsync())
                    {
                        results[resultKey].Add(await reader.GetFieldValueAsync<string>(0), await reader.GetFieldValueAsync<int>(1));
                    }
                    await reader.NextResultAsync();
                }
                reader.Dispose();
            }
            return new SummaryStats
            {
                Set1 = results["Set1KeyHeader"],
                Set2 = results["Set2KeyHeader"]
            };
        }

    }
}
