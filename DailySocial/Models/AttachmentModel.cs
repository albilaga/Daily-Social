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