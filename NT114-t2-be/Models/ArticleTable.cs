using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class ArticleTable
{
    public int ArticleId { get; set; }

    public string ArticleTitle { get; set; } = null!;

    public string Body { get; set; } = null!;

    public int AuthorId { get; set; }

    public int Views { get; set; }

    public int Likes { get; set; }

    public int Comments { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual UserTable Author { get; set; } = null!;

    public virtual ICollection<CommentTable> CommentTables { get; } = new List<CommentTable>();

    public virtual ICollection<UserArticleTable> UserArticleTables { get; } = new List<UserArticleTable>();

    public virtual ICollection<TagTable> Tags { get; } = new List<TagTable>();
}
