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
    public class RootTopStoriesModel
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public int Count_Total { get; set; }
        public int Pages { get; set; }
        public List<PostModel> Posts { get; set; }
        public List<PostModel> PostsByCategory { get; set; }
    }
}