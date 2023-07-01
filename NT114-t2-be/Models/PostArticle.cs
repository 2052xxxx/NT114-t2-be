namespace NT114_t2_be.Models
{
    public class PostArticle
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int AuthorId { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
