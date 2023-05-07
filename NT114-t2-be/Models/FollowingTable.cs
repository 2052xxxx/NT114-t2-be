using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class FollowingTable
{
    public int Id { get; set; }

    public int FollowerId { get; set; }

    public int FollowingId { get; set; }

    public virtual UserTable Follower { get; set; } = null!;

    public virtual UserTable Following { get; set; } = null!;
}
