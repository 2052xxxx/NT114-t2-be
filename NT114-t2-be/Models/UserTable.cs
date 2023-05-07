using System;
using System.Collections.Generic;

namespace NT114_t2_be.Models;

public partial class UserTable
{
    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Realname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Bio { get; set; }

    public DateTime RegistrationDate { get; set; }

    public virtual ICollection<ArticleTable> ArticleTables { get; } = new List<ArticleTable>();

    public virtual ICollection<CommentTable> CommentTables { get; } = new List<CommentTable>();

    public virtual ICollection<FollowingTable> FollowingTableFollowers { get; } = new List<FollowingTable>();

    public virtual ICollection<FollowingTable> FollowingTableFollowings { get; } = new List<FollowingTable>();

    public virtual ICollection<UserArticleTable> UserArticleTables { get; } = new List<UserArticleTable>();

    public virtual ICollection<TagTable> Tags { get; } = new List<TagTable>();
}
