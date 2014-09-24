using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DailySocial.Models
{
    public class RootCategoriesModel
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
    
}