using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool LoginAsAdmin { get; set; }
    }
}
