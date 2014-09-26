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
using Android.Graphics;

namespace DailySocial.Models
{
    public class DetailImageModel
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Bitmap Images { get; set; }
    }
}