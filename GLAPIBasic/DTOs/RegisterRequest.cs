using GLAPIBasic.Validations;
using System.ComponentModel.DataAnnotations;

namespace GLAPIBasic.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MailValidation(ErrorMessage = "Not a valid email address")]
        [StringLength(50)]
        public string? UserName { get; set; } 
        //
        // 摘要:
        //     The code sent to the user's email to reset the password. To get the reset code,
        //     first make a "/forgotPassword" request.
        [Required(ErrorMessage = "ResetCode is required")]
        [StringLength(5)]
        [RegularExpression(@"\d{5}")]
        public required string ResetCode { get; init; }


        [StringLength(50)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least 8 characters, including uppercase, lowercase letters and numbers")]
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; init; }

        [StringLength(50)]
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// 头像缩略图，base64编码，保存时解压成byte[]
        /// </summary>
        public string AvatarThumbnail { get; set; } = null!;
    }
}
