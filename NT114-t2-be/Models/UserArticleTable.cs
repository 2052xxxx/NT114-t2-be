using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class UserArticleTable
{
    public int UserId { get; set; }

    public int ArticleId { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual ArticleTable Article { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
