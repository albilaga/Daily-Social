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
using System.Threading.Tasks;

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
            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            if (_DataArticlesByCategory != null)
            {
                _DataArticlesByCategory = null;
            }
            
            _DataArticlesByCategory = new TopStoriesViewModel();
            
            if (_DataArticlesByCategory.Posts != null)
            {
                _DataArticlesByCategory.Posts.Clear();
                _DataArticlesByCategory.Posts = null;
            }
            string idx = Intent.GetStringExtra("IdFromCategories");
            int id = Int16.Parse(idx);
            Log.Info("ds", "id on articles = " + id);
            if(_ArticlesByCategoryDownloader!=null)
            {
                _ArticlesByCategoryDownloader = null;
            }
            _ArticlesByCategoryDownloader = new DataService();
            _ArticlesByCategoryDownloader.GetArticlesByCategory(id);
            _ArticlesByCategoryDownloader.DownloadCompleted += _ArticlesByCategoryDownloader_DownloadCompleted;
        }

        public override void OnBackPressed()
        {
            //this.StartActivity(typeof(MainActivity));
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
                    Task.Factory.StartNew(() =>
                        {
                            _DataArticlesByCategory = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                            int index = 0;
                            foreach (var x in _DataArticlesByCategory.Posts)
                            {
                                Log.Info("ds", "index = " + index);
                                //Log.Info
                                if (x.Attachments.Count > 0)
                                {
                                    x.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(x.Attachments[0].Images.Full.Url);
                                }
                                index++;
                            }
                            Log.Info("ds", "bitmap downloaded");
                            _IsLoaded = true;
                            Log.Info("ds", "fragment articles by categories visible");
                        }).ContinueWith(todo =>
                            {
                                if (_IsLoaded)
                                {
                                    RunOnUiThread(() =>
                                    {
                                        _ListView.Adapter = new TopStoriesAdapter(this, _DataArticlesByCategory.Posts);
                                        _ProgressBar.Activated = false;
                                    });
                                    Log.Info("ds", "top stories downloaded");
                                }
                            });
                }
            }
        }
    }
}