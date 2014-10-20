using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
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
    [Activity(Theme = "@style/Theme.ActionBarGreen")]
    public class ArticlesByCategoryActivity : Activity
    {
        private ListView _ListView;
        private ProgressBar _ProgressBar;
        private TextView _TextView;
        private DataService _ArticlesByCategoryDownloader;
        private TopStoriesViewModel _DataArticlesByCategory;
        private bool _IsLoaded;
        private int _Id;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            string title = Intent.GetStringExtra("TitleFromCategories");
            Title = title;

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            //UI Layout
            SetContentView(Resource.Layout.ListLayout);
            _ListView = FindViewById<ListView>(Resource.Id.ListView);
            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            _TextView = FindViewById<TextView>(Resource.Id.TextView);
            _TextView.Visibility = ViewStates.Gone;

            _ListView.ItemClick += OnItemClick;

            _DataArticlesByCategory = new TopStoriesViewModel();

            //get id from categories
            var idx = Intent.GetStringExtra("IdFromCategories");
            _Id = int.Parse(idx);

            _ArticlesByCategoryDownloader = new DataService();
            _ArticlesByCategoryDownloader.GetArticlesByCategory(_Id);
            _ArticlesByCategoryDownloader.DownloadCompleted += _ArticlesByCategoryDownloader_DownloadCompleted;
            RunOnUiThread(ShowInternetAlertDialog);
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(BaseContext, typeof(DetailArticleActivity));
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
            FinishThisActivity();
            base.OnBackPressed();
        }

        private void FinishThisActivity()
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
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    FinishThisActivity();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowInternetAlertDialog()
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetIcon(Resource.Drawable.error);
            builder.SetTitle("Jaringan Bermasalah");
            builder.SetMessage("Apakah anda ingin mencoba download data lagi?");
            builder.SetCancelable(false);
            builder.SetPositiveButton("Ya", (send, eve) =>
            {
                _ArticlesByCategoryDownloader.DownloadCompleted += _ArticlesByCategoryDownloader_DownloadCompleted;
                _ArticlesByCategoryDownloader.GetDetailArticle(_Id);
                Reset();
            });
            builder.SetNegativeButton("Tidak", (send, eve) => ShowList());
            var alertDialog = builder.Create();
            var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection == null) || !activeConnection.IsConnected)
            {
                alertDialog.Show();
            }
        }

        private void Reset()
        {
            RunOnUiThread(() =>
            {
                _ProgressBar.Visibility = ViewStates.Visible;
                _ProgressBar.Activated = true;
            });
        }

        private void ShowList()
        {
            if (_IsLoaded)
            {
                RunOnUiThread(() =>
                {
                    _ListView.Adapter = new TopStoriesAdapter(this, _DataArticlesByCategory.Posts);
                });
            }
            RunOnUiThread(() =>
            {
                _ProgressBar.Visibility = ViewStates.Gone;
            });
        }

        private void _ArticlesByCategoryDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            _ArticlesByCategoryDownloader.DownloadCompleted -= _ArticlesByCategoryDownloader_DownloadCompleted;
            if (((DownloadEventArgs)e).ResultDownload == null) return;
            var raw = ((DownloadEventArgs)e).ResultDownload;
            if (raw.Length != 0)
            {
                Task.Factory.StartNew(() =>
                {
                    _DataArticlesByCategory = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                    _IsLoaded = true;
                    ShowList();
                });
            }
        }
    }
}