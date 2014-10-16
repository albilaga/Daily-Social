using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DailySocial.View
{
    [Activity(Label = "Detail Article", Theme = "@style/Theme.AppCompat.Light")]
    public class DetailArticleActivity : ActionBarActivity
    {
        //private ImageView _ImageTitleHolder;
        //private TextView _ContentTextView;
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

            // Create your application here
            SetContentView(Resource.Layout.DetailArticleLayout);
            //_ImageTitleHolder = FindViewById<ImageView>(Resource.Id.ImageTitleHolder);
            //_ContentTextView = FindViewById<TextView>(Resource.Id.ContentTextView);
            _Progressbar = FindViewById<ProgressBar>(Resource.Id.ProgressBarOnDetail);
            _Bookmarks = ListUtils.LoadBookmarks();
            _WebView = FindViewById<WebView>(Resource.Id.WebView);
            _WebView.Visibility = ViewStates.Invisible;
            _WebView.Settings.JavaScriptEnabled = false;
            _WebView.Settings.LoadWithOverviewMode = true;
            _WebView.Settings.UseWideViewPort = true;
            _WebView.Settings.BuiltInZoomControls = false;

            string ids = Intent.GetStringExtra("IdForDetail");
            int id = int.Parse(ids);

            _DetailArticleDownloader = new DataService();
            _DetailArticleDownloader.GetDetailArticle(id);
            _DetailArticleDownloader.DownloadCompleted += OnDownloadCompleted;

            _DataArticle = new ArticleViewModel();
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
                    foreach (var x in _Bookmarks.Bookmarks)
                    {
                        if (_DataArticle.Post.Id == x.Id)
                        {
                            _DataArticle.Post.IsFavorite = true;
                            _BookmarksId = x.Id;
                            break;
                        }
                    }
                    if (_DataArticle.Post.IsFavorite)
                    {
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
                        if (!_DataArticle.Post.IsFavorite)
                        {
                            ListUtils.SaveBookmarks(_DataArticle.Post);
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
                        }
                        else
                        {
                            var get = _Bookmarks.Bookmarks.Where(w => w.Id == _BookmarksId).FirstOrDefault();
                            if (get == null)
                            {
                            }
                            else
                            {
                                _Bookmarks.Bookmarks.Remove(get);
                            }
                            item.SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_not_important));
                        }
                        break;
                    }
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
            BookmarksFragment bookmarksFragment = new BookmarksFragment();
            bookmarksFragment.Bookmarks = ListUtils.LoadBookmarks();
            Finish();
            base.OnBackPressed();
        }

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            _DetailArticleDownloader.DownloadCompleted -= OnDownloadCompleted;
            if (((Utils.DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((Utils.DownloadEventArgs)e).ResultDownload;
                _DataArticle = JsonConvert.DeserializeObject<ArticleViewModel>(raw);
                //_DataArticle.Post.Title = HttpUtility.HtmlDecode(_DataArticle.Post.Title);
                //_DataArticle.Post.Content = HttpUtility.HtmlDecode(_DataArticle.Post.Content);
                //if (_DataArticle.Post.Attachments.Count > 0)
                //{
                //    _DataArticle.Post.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(_DataArticle.Post.Attachments[0].Images.Full.Url);
                //}
                RunOnUiThread(() =>
                    {
                        InvalidateOptionsMenu();
                        _Progressbar.Visibility = ViewStates.Gone;
                        //_ImageTitleHolder.SetImageBitmap(_DataArticle.Post.Attachments[0].Images.Full.Images);
                        //_ContentTextView.Text = _DataArticle.Post.Content.Trim();
                        _WebView.LoadDataWithBaseURL("", _DataArticle.Post.Content, "text/html", "UTF-8", "");
                        _WebView.Visibility = ViewStates.Visible;
                    });
            }
        }
    }
}