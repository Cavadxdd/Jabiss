using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Account
{
    public class VerifyCodeViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 digits")]
        public string Code { get; set; }
    }
}
