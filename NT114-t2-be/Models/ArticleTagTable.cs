using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class ArticleTagTable
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int TagId { get; set; }

    public virtual ArticleTable Article { get; set; } = null!;

    public virtual TagTable Tag { get; set; } = null!;
}
