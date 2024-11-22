using System;
using System.Collections.Generic;

namespace GLAPIBasic.Models;

public partial class User
{
    public long UserId { get; set; }

    public string? Username { get; set; }

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public byte[]? AvatarThumbnail { get; set; }
}
