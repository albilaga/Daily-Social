using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs;
using DailySocial.ViewModel;
using Newtonsoft.Json;

namespace DailySocial.View
{
    [Activity(Label = "Detail Article", Theme = "@style/Theme.AppCompat.Light")]
    public class DetailArticleActivity : ActionBarActivity
    {
        private ProgressBar _Progressbar;


        private DataService _DetailArticleDownloader;

        private ArticleViewModel _DataArticle;
        private Android.Support.V7.Widget.ShareActionProvider _ShareAction;
        private WebView _WebView;
        private BookmarksViewModel _Bookmarks;
        private int _BookmarksId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.DetailArticleLayout);

            _Progressbar = FindViewById<ProgressBar>(Resource.Id.ProgressBarOnDetail);
            _Bookmarks = ListUtils.LoadBookmarks();
            _WebView = FindViewById<WebView>(Resource.Id.WebView);
            _WebView.Visibility = ViewStates.Invisible;
            _WebView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);

            string ids = Intent.GetStringExtra("IdForDetail");
            int id = int.Parse(ids);

            string dataRaw = Intent.GetStringExtra("DataRaw");
            Log.Debug("ds", dataRaw);
           

            _DetailArticleDownloader = new DataService();
            _DetailArticleDownloader.GetDetailArticle(id);
            _DetailArticleDownloader.DownloadCompleted += OnDownloadCompleted;

            _DataArticle = new ArticleViewModel();
            //ParseData(dataRaw);
            _Bookmarks = ListUtils.LoadBookmarks() ?? new BookmarksViewModel();
        }

        private void ParseData(string raw)
        {
            _DataArticle = JsonConvert.DeserializeObject<ArticleViewModel>(raw);
            RunOnUiThread(() =>
            {
                InvalidateOptionsMenu();
                _Progressbar.Visibility = ViewStates.Gone;
                _WebView.LoadDataWithBaseURL("", getHtmlData(_DataArticle.Post.Content), "text/html", "UTF-8", "");
                _WebView.Visibility = ViewStates.Visible;
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);

            if (_DataArticle.Post == null)
            {
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(false);
                menu.FindItem(Resource.Id.Bookmarks).SetVisible(false);
            }
            else
            {
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(true);
                menu.FindItem(Resource.Id.Bookmarks).SetVisible(true);
                if (_Bookmarks != null)
                {
                    var get = _Bookmarks.Bookmarks.FirstOrDefault(x => _DataArticle.Post.Id == x.Id);
                    if (get != null)
                    {
                        _BookmarksId = get.Id;
                        menu.FindItem(Resource.Id.Bookmarks).SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
                    }
                    else
                    {
                        menu.FindItem(Resource.Id.Bookmarks).SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important));
                    }
                }
                else
                {
                    menu.FindItem(Resource.Id.Bookmarks).SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important));
                }
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
                        var get = _Bookmarks.Bookmarks.FirstOrDefault(w => w.Id == _BookmarksId);
                        if (get == null)
                        {
                            ListUtils.SaveBookmarks(_DataArticle.Post);
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
                        }
                        else
                        {
                            _Bookmarks.Bookmarks.Remove(get);
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important));
                        }
                    }
                    break;
            }

            base.OnOptionsItemSelected(item);
            return true;
        }

        public override void OnBackPressed()
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
            // ReSharper disable ObjectCreationAsStatement
            new BookmarksFragment { Bookmarks = ListUtils.LoadBookmarks() };
            // ReSharper restore ObjectCreationAsStatement
            Finish();
            base.OnBackPressed();
        }

        private string getHtmlData(string bodyHtml)
        {
            const string head = "<head><style>img{max-width: 100%; width:auto; height: auto;}</style></head>";
            return "<html>" + head + "<body>" + bodyHtml + "</body></html>";
        }

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            _DetailArticleDownloader.DownloadCompleted -= OnDownloadCompleted;
            if (((Utils.DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((Utils.DownloadEventArgs)e).ResultDownload;
                _DataArticle = JsonConvert.DeserializeObject<ArticleViewModel>(raw);
                RunOnUiThread(() =>
                    {
                        InvalidateOptionsMenu();
                        _Progressbar.Visibility = ViewStates.Gone;
                        _WebView.LoadDataWithBaseURL("", getHtmlData(_DataArticle.Post.Content), "text/html", "UTF-8", "");
                        _WebView.Visibility = ViewStates.Visible;
                    });
            }
        }
    }
}