namespace GLAPIBasic.DTOs
{
    public class GetUserInfoResponse
    { 
        public string Username { get; set; } = null!; 

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? AvatarThumbnail { get; set; }
    }
}
