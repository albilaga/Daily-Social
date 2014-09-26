using Newtonsoft.Json;
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
using DailySocial.Models;
using DailySocial.View.Tabs.Adapter;
using DailySocial.Utils;
using DailySocial.ViewModel;
using Android.Util;

namespace DailySocial.View.Tabs
{
    [Activity(Label = "TopStoriesActivity")]
    public class TopStoriesActivity : ListActivity
    {
        public TopStoriesViewModel DataTopStories { get; private set; } 
        private DataService _DataService;
        private ProgressBar _ProgressBar;
        //private int _IndexCount
        
        protected override void OnCreate(Bundle bundle)
        {
            DataTopStories = new TopStoriesViewModel();
            _DataService = new DataService();

            SetContentView(Resource.Layout.ListLayout);
            _DataService.GetTopStories();
            _DataService.DownloadCompleted += _DataService_DownloadCompleted;

            //When data still null enable loading 
            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            if(DataTopStories==null)
            {
                _ProgressBar.Activated = true;
            }
            else
            { }

            base.OnCreate(bundle);
        }


        void _DataService_DownloadCompleted(object sender, EventArgs e)
        {
            var raw = ((DownloadEventArgs)e).ResultDownload;
            if(raw!=null)
            {
                DataTopStories = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                int index=0;
                foreach(var x in DataTopStories.Posts)
                {
                    Log.Info("ds", "index = " + index);
                    x.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(x.Attachments[0].Images.Full.Url);
                    index++;
                }
                Log.Info("ds", "top stories downloaded");
                //Update List View on UI Thread
                RunOnUiThread(() => 
                    {
                        ListAdapter = new TopStoriesAdapter(this, DataTopStories.Posts);
                        _ProgressBar.Activated = false;
                    });
               
            }
        }

    }
}