using DailySocial.Utils;
using System.Collections.Generic;
using System.Web;

namespace DailySocial.Models
{
    public class PostModel
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Slug { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public string Title { get; set; }

        public string TitleDecode
        {
            get
            {
                return HttpUtility.HtmlDecode(Title);
            }
        }

        public string Title_Plain { get; set; }

        public string Content { get; set; }

        public string Excerpt { get; set; }

        public string Date { get; set; }

        public string Modified { get; set; }

        public List<CategoryModel> Categories { get; set; }

        public List<TagModel> Tags { get; set; }

        public AuthorModel Author { get; set; }

        public List<object> Comments { get; set; }

        public List<AttachmentModel> Attachments { get; set; }

        public int Comment_Count { get; set; }

        public string Comment_Status { get; set; }

        public string LongDateTime
        {
            get
            {
                return ListUtils.DateTimeToDateConverter(Date);
            }
        }

        public string LongTime
        {
            get { return ListUtils.DateTimeToTimeConverter(Date); }
        }
    }
}