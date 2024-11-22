using GLAPIBasic.Validations;
using System.ComponentModel.DataAnnotations;

namespace GLAPIBasic.DTOs
{
    public class SendCodeRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [MailValidation(ErrorMessage = "Not a valid email address")]
        [StringLength(50)]
        public required string Email { get; init; }
    }
}
