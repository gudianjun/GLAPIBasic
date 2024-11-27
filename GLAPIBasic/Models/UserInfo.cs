using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLAPIBasic.Models;

public partial class UserInfo
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 标记为自增字段
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? AvatarThumbnail { get; set; }
}
