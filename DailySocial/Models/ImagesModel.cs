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
    public class ImagesModel
    {
        public DetailImageModel Full { get; set; }
        public DetailImageModel Thumbnail { get; set; }
        public DetailImageModel Medium { get; set; }
        public DetailImageModel Large { get; set; }
    }
}