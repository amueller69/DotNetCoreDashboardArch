using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreDashboardArch.Models.Reports
{
    public class ReportParms
    {
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string Name { get; set; }
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string Type { get; set; }
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string PolicyID { get; set; }
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string Status { get; set; }
    }
}
