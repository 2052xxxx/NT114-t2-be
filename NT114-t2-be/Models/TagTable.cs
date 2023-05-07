using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class TagTable
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ArticleTagTable> ArticleTagTables { get; } = new List<ArticleTagTable>();

    public virtual ICollection<UserTable> Users { get; } = new List<UserTable>();
}
