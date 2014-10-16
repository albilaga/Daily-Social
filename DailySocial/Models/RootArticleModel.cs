namespace DailySocial.Models
{
    public class RootArticleModel
    {
        public string Status { get; set; }

        public PostModel Post { get; set; }

        public string Previous_Url { get; set; }
    }
}