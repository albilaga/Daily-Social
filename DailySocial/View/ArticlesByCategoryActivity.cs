using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs.Adapter;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DailySocial.View
{
    [Activity(Theme = "@style/Theme.AppCompat.Light")]
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

            string title = Intent.GetStringExtra("TitleFromCategories");
            Title = title;

            //UI Layout
            SetContentView(Resource.Layout.ListLayout);
            _ListView = FindViewById<ListView>(Resource.Id.ListView);
            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);

            _ListView.ItemClick += OnItemClick;

            _DataArticlesByCategory = new TopStoriesViewModel();

            //get id from categories
            string idx = Intent.GetStringExtra("IdFromCategories");
            int id = int.Parse(idx);
            Log.Info("ds", "id on articles = " + id);

            _ArticlesByCategoryDownloader = new DataService();
            _ArticlesByCategoryDownloader.GetArticlesByCategory(id);
            _ArticlesByCategoryDownloader.DownloadCompleted += _ArticlesByCategoryDownloader_DownloadCompleted;
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(BaseContext, typeof(DetailArticleActivity));
            intent.PutExtra("IdForDetail", e.Id.ToString(CultureInfo.InvariantCulture));
            StartActivity(intent);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// override method when back key pressed and dispose and recycle all data used in this activity
        /// </summary>
        public override void OnBackPressed()
        {
            _ArticlesByCategoryDownloader = null;
            if (_DataArticlesByCategory.Posts != null)
            {
                _DataArticlesByCategory.Posts.Clear();
                _DataArticlesByCategory.Posts = null;
            }
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_DataArticlesByCategory != null)
            {
                _DataArticlesByCategory = null;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finish();
            base.OnBackPressed();
        }

        private void _ArticlesByCategoryDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            _ArticlesByCategoryDownloader.DownloadCompleted -= _ArticlesByCategoryDownloader_DownloadCompleted;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                string raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw.Length != 0)
                {
                    Task.Factory.StartNew(() =>
                        {
                            _DataArticlesByCategory = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                            //foreach (var x in _DataArticlesByCategory.Posts)
                            //{
                            //    x.Title = HttpUtility.HtmlDecode(x.Title);
                            //    Log.Info("ds", "index = " + index);
                            //    //Log.Info
                            //    if (x.Attachments.Count > 0)
                            //    {
                            //        x.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(x.Attachments[0].Images.Medium.Url);
                            //    }
                            //    index++;
                            //}
                            Log.Info("ds", "bitmap downloaded");
                            _IsLoaded = true;
                            Log.Info("ds", "fragment articles by categories visible");
                        }).ContinueWith(todo =>
                            {
                                if (todo.Status == TaskStatus.RanToCompletion)
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
                                }
                            });
                }
            }
        }
    }
}