namespace GLAPIBasic.DTOs
{
    public class UserInfo
    {
        public long UserId { get; set; }
        public string? Address { get; set; }
        public string Name { get; set; } = null!;
        public string? CompanyName { get; set; }
        public string? AvatarIcon { get; set; }
    }
}
