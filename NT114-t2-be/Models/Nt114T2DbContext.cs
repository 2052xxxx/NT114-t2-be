using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NT114_t2_be.Models;

public partial class Nt114T2DbContext : DbContext
{
    public Nt114T2DbContext()
    {
    }

    public Nt114T2DbContext(DbContextOptions<Nt114T2DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArticleTable> ArticleTables { get; set; }

    public virtual DbSet<ArticleTagTable> ArticleTagTables { get; set; }

    public virtual DbSet<CommentTable> CommentTables { get; set; }

    public virtual DbSet<FollowingTable> FollowingTables { get; set; }

    public virtual DbSet<TagTable> TagTables { get; set; }

    public virtual DbSet<UserArticleTable> UserArticleTables { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = .\\SQLEXPRESS; Database=NT114_t2_db;TrustServerCertificate=true; Trusted_Connection=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleTable>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__ArticleT__CC36F66039619532");

            entity.ToTable("ArticleTable");

            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.ArticleTitle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("article_title");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Body)
                .HasColumnType("text")
                .HasColumnName("body");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.LastUpdated)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("last_updated");
            entity.Property(e => e.Likes).HasColumnName("likes");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.Author).WithMany(p => p.ArticleTables)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ArticleTa__autho__4AB81AF0");
        });

        modelBuilder.Entity<ArticleTagTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Article___3213E83F291E0D5A");

            entity.ToTable("Article_tagTable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleTagTables)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article_t__artic__43D61337");

            entity.HasOne(d => d.Tag).WithMany(p => p.ArticleTagTables)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article_t__tag_i__44CA3770");
        });

        modelBuilder.Entity<CommentTable>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__CommentT__E79576875BD5589F");

            entity.ToTable("CommentTable");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Text)
                .HasColumnType("text")
                .HasColumnName("text");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Article).WithMany(p => p.CommentTables)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentTa__artic__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.CommentTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentTa__user___628FA481");
        });

        modelBuilder.Entity<FollowingTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Followin__3213E83F8E003596");

            entity.ToTable("FollowingTable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.FollowingId).HasColumnName("following_id");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowingTableFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Following__follo__160F4887");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowingTableFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Following__follo__17036CC0");
        });

        modelBuilder.Entity<TagTable>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__TagTable__4296A2B62081B3FC");

            entity.ToTable("TagTable");

            entity.HasIndex(e => e.TagName, "UQ__TagTable__E298655CADB9932E").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<UserArticleTable>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ArticleId }).HasName("PK__User_Art__A57D58695F3E4DFF");

            entity.ToTable("User_ArticleTable");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Timestamp)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Article).WithMany(p => p.UserArticleTables)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Arti__artic__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.UserArticleTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Arti__user___74AE54BC");
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__UserTabl__CBA1B25768C743E1");

            entity.ToTable("UserTable");

            entity.HasIndex(e => e.Email, "UQ__UserTabl__AB6E6164FFF86048").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__UserTabl__F3DBC5722E08A6BB").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Bio)
                .HasColumnType("text")
                .HasColumnName("bio");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Realname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("realname");
            entity.Property(e => e.RegistrationDate)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("registration_date");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasMany(d => d.Tags).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserTagTable",
                    r => r.HasOne<TagTable>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__User_tagT__tag_i__70DDC3D8"),
                    l => l.HasOne<UserTable>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__User_tagT__user___6FE99F9F"),
                    j =>
                    {
                        j.HasKey("UserId", "TagId").HasName("PK__User_tag__CD975D24FCC8BA36");
                        j.ToTable("User_tagTable");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
