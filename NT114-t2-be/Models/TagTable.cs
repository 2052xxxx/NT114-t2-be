using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class TagTable
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ArticleTable> Articles { get; } = new List<ArticleTable>();

    public virtual ICollection<UserTable> Users { get; } = new List<UserTable>();
}
