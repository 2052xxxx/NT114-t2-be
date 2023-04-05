using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class CommentTable
{
    public int CommentId { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ArticleTable Article { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
