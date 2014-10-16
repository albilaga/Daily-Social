using System.Collections.Generic;

namespace DailySocial.Models
{
    public class RootCategoriesModel
    {
        public string Status { get; set; }

        public int Count { get; set; }

        public List<CategoryModel> Categories { get; set; }
    }
}