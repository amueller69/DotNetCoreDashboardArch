using System.ComponentModel.DataAnnotations;

namespace DotNetCoreDashboardArch.Models.Login
{
    public class LoginModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
