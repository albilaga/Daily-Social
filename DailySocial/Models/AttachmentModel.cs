namespace DailySocial.Models
{
    public class AttachmentModel
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Caption { get; set; }

        public int Parent { get; set; }

        public string Mime_type { get; set; }

        public ImagesModel Images { get; set; }
    }
}