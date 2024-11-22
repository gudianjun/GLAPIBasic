using System;
using System.Collections.Generic;

namespace GLAPIBasic.Models;

public partial class UserHistory
{
    public long UserId { get; set; }

    public long SeqNum { get; set; }

    public DateTime? LoginDatetime { get; set; }

    public string? IpAddress { get; set; }
}
