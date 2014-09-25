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

namespace DailySocial.Utils
{
    public class DownloadEventArgs :EventArgs
    {
        public string ResultDownload { get; set; }
    }
}