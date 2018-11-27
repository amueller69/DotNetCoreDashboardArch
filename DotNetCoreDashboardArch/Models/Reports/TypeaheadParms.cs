using System.ComponentModel.DataAnnotations;
namespace DotNetCoreDashboardArch.Models.Reports
{
    public class TypeaheadParms
    {
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string Name { get; set; }
        [RegularExpression(@"^(?!.*[;\""\']+)(?!.*-{2,})(?!.*(/\\\*|\*/)).*$")]
        public string Value { get; set; }
    }
}
