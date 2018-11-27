using System.Collections.Generic;


namespace DotNetCoreDashboardArch.Models.Reports
{
    public class ReportResults
    {
        public List<Dictionary<string, object>> ResultsList { get; set; }
        public int ItemsCount { get; set; } 
    }
}
