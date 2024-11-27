using GLAPIBasic.Validations;
using System.ComponentModel.DataAnnotations;

namespace GLAPIBasic.DTOs
{
    public class SendResetPasswordCodeRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        [MailValidation(ErrorMessage = "Not a valid email address")]
        [StringLength(50)]
        public required string UserName { get; init; }
    }
}
