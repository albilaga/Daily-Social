using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using DailySocial.Models;
using DailySocial.Utils;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DailySocial.View
{
    [Activity(Theme = "@style/Theme.ActionBarGreen", Label = "Berita")]
    public class DetailArticleActivity : ActionBarActivity
    {
        private ProgressBar _Progressbar;

        private DataService _DetailArticleDownloader;

        private ArticleViewModel _DataArticle;
        private Android.Support.V7.Widget.ShareActionProvider _ShareAction;
        private WebView _WebView;
        private TextView _TitleTextView;
        private TextView _AuthorAndDateTextView;

        private BookmarksViewModel _Bookmarks;
        private PostModel _SelectedBookmarks;
        private int _Id;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            SetContentView(Resource.Layout.DetailArticleLayout);

            _Progressbar = FindViewById<ProgressBar>(Resource.Id.ProgressBarOnDetail);
            _TitleTextView = FindViewById<TextView>(Resource.Id.TitleTextView);
            _AuthorAndDateTextView = FindViewById<TextView>(Resource.Id.AuthorAndDateTimeTextView);
            _WebView = FindViewById<WebView>(Resource.Id.WebView);
            _WebView.Visibility = ViewStates.Invisible;
            _WebView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);

            string ids = Intent.GetStringExtra("IdForDetail");
            _Id = int.Parse(ids);

            _DataArticle = new ArticleViewModel();
            //ParseData(dataRaw);
            _Bookmarks = ListUtils.LoadBookmarks() ?? new BookmarksViewModel();
            _SelectedBookmarks = _Bookmarks.Bookmarks.FirstOrDefault(x => _Id == x.Id);
            //if data on bookmarks directly show it up
            if (_SelectedBookmarks != null)
            {
                _DataArticle.Post = _SelectedBookmarks;
                ShowList();
            }
            //if not found in bookmarks then download it first
            else
            {
                DownloadData();
            }
        }

        private void DownloadData()
        {
            _DetailArticleDownloader = new DataService();
            _DetailArticleDownloader.GetDetailArticle(_Id);
            _DetailArticleDownloader.DownloadCompleted += OnDownloadCompleted;
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
                _DetailArticleDownloader.DownloadCompleted += OnDownloadCompleted;
                _DetailArticleDownloader.GetDetailArticle(_Id);
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
                _Progressbar.Visibility = ViewStates.Visible;
                _Progressbar.Activated = true;
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);

            //when downloading data can not share and bookmarks icon set invisible
            if (_DataArticle.Post == null)
            {
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(false);
                menu.FindItem(Resource.Id.Bookmarks).SetEnabled(false);
            }
            //if already finish downloading data
            else
            {
                //show action bar icon
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(true);
                menu.FindItem(Resource.Id.Bookmarks).SetVisible(true);
                menu.FindItem(Resource.Id.Bookmarks)
                    .SetIcon(_SelectedBookmarks == null
                    //if the detail article is not bookmarked
                        ? Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important)
                    //if the detail article is bookmarked
                        : Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
                var shareItem = menu.FindItem(Resource.Id.ActionShare);
                var shareItemAction = MenuItemCompat.GetActionProvider(shareItem);
                _ShareAction = shareItemAction.JavaCast<Android.Support.V7.Widget.ShareActionProvider>();
                var intent = new Intent(Intent.ActionSend);
                intent.SetType("text/plain");
                intent.PutExtra(Intent.ExtraSubject, _DataArticle.Post.Title);
                intent.PutExtra(Intent.ExtraText, _DataArticle.Post.Url);
                _ShareAction.SetShareIntent(intent);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Bookmarks:
                    {
                        if (_SelectedBookmarks == null)
                        {
                            _SelectedBookmarks = _DataArticle.Post;
                            _Bookmarks.Bookmarks.Add(_SelectedBookmarks);
                            ListUtils.SaveBookmarks(_Bookmarks);
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
                            Toast.MakeText(this, "Anda berhasil memfavoritkan halaman ini", ToastLength.Short);
                        }
                        else
                        {
                            _Bookmarks.Bookmarks.Remove(_SelectedBookmarks);
                            ListUtils.SaveBookmarks(_Bookmarks);
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important));
                            Toast.MakeText(this, "Anda berhasil menghapus halaman favorit ini", ToastLength.Short);
                        }
                    }
                    break;

                case Android.Resource.Id.Home:
                    FinishThisActivity();
                    break;
            }

            base.OnOptionsItemSelected(item);
            return true;
        }

        public override void OnBackPressed()
        {
            FinishThisActivity();
            base.OnBackPressed();
        }

        private void FinishThisActivity()
        {
            if (_DataArticle != null)
            {
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (_DataArticle.Post != null)
                {
                    _DataArticle.Post = null;
                }
                _DataArticle = null;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finish();
        }

        private void ShowList()
        {
            RunOnUiThread(() =>
            {
                InvalidateOptionsMenu();
                _Progressbar.Visibility = ViewStates.Gone;
                _TitleTextView.Text = _DataArticle.Post.TitleDecode;
                _AuthorAndDateTextView.Text = string.Format("{0} | {1} | {2}", _DataArticle.Post.Author.Name, _DataArticle.Post.LongDateTime, _DataArticle.Post.LongTime);
                _WebView.LoadDataWithBaseURL("", GetHtmlData(_DataArticle.Post.Content), "text/html", "UTF-8", "");
                _WebView.Visibility = ViewStates.Visible;
            });
        }

        private static string GetHtmlData(string bodyHtml)
        {
            const string head = "<head><style>img{max-width: 100%; width:auto; height: auto;}</style></head>";
            return string.Format("<html>{0}<body>{1}</body></html>", head, bodyHtml);
        }

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            _DetailArticleDownloader.DownloadCompleted -= OnDownloadCompleted;
            if (((Utils.DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((Utils.DownloadEventArgs)e).ResultDownload;
                _DataArticle = JsonConvert.DeserializeObject<ArticleViewModel>(raw);
                ShowList();
            }
            else
            {
                RunOnUiThread(ShowInternetAlertDialog);
            }
        }
    }
}