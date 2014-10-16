using System.Collections.Generic;

namespace DailySocial.Models
{
    public class RootPostsByCategoryModel
    {
        public string Status { get; set; }

        public int Count { get; set; }

        public int Pages { get; set; }

        public CategoryModel Category { get; set; }

        public List<PostModel> Posts { get; set; }
    }
}