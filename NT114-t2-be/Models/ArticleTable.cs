namespace NT114_t2_be.Models
{
    public class ArticleTable
    {
        public int articleId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string authorId { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
        public string comments { get; set; }
        public string lastUpdate { get; set; }


    }
}
