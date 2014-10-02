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
using DailySocial.Utils;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using Android.Util;
using DailySocial.View.Tabs.Adapter;

namespace DailySocial.View
{
    [Activity(Label = "ArticlesByCategory")]
    public class ArticlesByCategoryActivity : Activity
    {
        private ListView _ListView;
        private ProgressBar _ProgressBar;
        private DataService _ArticlesByCategoryDownloader;
        private TopStoriesViewModel _DataArticlesByCategory;
        private bool _IsLoaded;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ListLayout);
            _ListView = FindViewById<ListView>(Resource.Id.ListView);

            if(_DataArticlesByCategory==null)
            {
                _DataArticlesByCategory = new TopStoriesViewModel();
            }

            int id = 9;
            _ArticlesByCategoryDownloader = new DataService();
            _ArticlesByCategoryDownloader.GetArticlesByCategory(id);
            _ArticlesByCategoryDownloader.DownloadCompleted += _ArticlesByCategoryDownloader_DownloadCompleted;
        }

        public override void OnBackPressed()
        {
            this.StartActivity(typeof(MainActivity));
            this.Finish();
            base.OnBackPressed();
        }


        void _ArticlesByCategoryDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            _ArticlesByCategoryDownloader.DownloadCompleted -= _ArticlesByCategoryDownloader_DownloadCompleted;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                string raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw.Length != 0 && raw!=null)
                {
                    _DataArticlesByCategory = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                    int index = 0;
                    foreach (var x in _DataArticlesByCategory.Posts)
                    {
                        if (!this.IsFinishing)
                        {
                            Log.Info("ds", "index = " + index);
                            //Log.Info
                            x.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(x.Attachments[0].Images.Full.Url);
                            index++;
                        }
                    }
                    Log.Info("ds", "bitmap downloaded");
                    _IsLoaded = true;
                    Log.Info("ds", "fragment top stories visible");
                    if (_IsLoaded)
                    {
                        RunOnUiThread(() =>
                        {
                            _ListView.Adapter = new TopStoriesAdapter(this, _DataArticlesByCategory.Posts);
                            _ProgressBar.Activated = false;
                        });
                        Log.Info("ds", "top stories downloaded");
                    }
                }
            }
        }
    }
}