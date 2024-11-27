namespace GLAPIBasic.DTOs
{
    public class LoginUserInfo
    {
        public long UserId { get; set; }

        public string Username { get; set; } = null!;  

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string AvatarThumbnail { get; set; } = null!;
    }
}
