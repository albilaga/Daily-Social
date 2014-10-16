using System.Collections.Generic;

namespace DailySocial.Models
{
    public class RootTopStoriesModel
    {
        public string Status { get; set; }

        public int Count { get; set; }

        public int Count_Total { get; set; }

        public int Pages { get; set; }

        public List<PostModel> Posts { get; set; }

        public List<PostModel> TempPosts { get; set; }

        public List<PostModel> PostsByCategory { get; set; }
    }
}