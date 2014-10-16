namespace DailySocial.Models
{
    public class TagModel
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Post_Count { get; set; }
    }
}