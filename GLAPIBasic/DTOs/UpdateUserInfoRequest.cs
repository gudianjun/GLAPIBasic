using System.ComponentModel.DataAnnotations;

namespace GLAPIBasic.DTOs
{
    public class UpdateUserInfoRequest
    { 
        [StringLength(50)] 
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        public string? AvatarThumbnail { get; set; }
    }
}
