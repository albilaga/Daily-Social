using Newtonsoft.Json;
using DailySocial.Utils;
using DailySocial.ViewModel;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using Android.Webkit;
using System.Web;

namespace DailySocial.View
{
    [Activity(Label = "Detail Article", Theme = "@style/Theme.AppCompat")]
    public class DetailArticleActivity : ActionBarActivity
    {
        private ImageView _ImageTitleHolder;
        private TextView _ContentTextView;
        private ProgressBar _Progressbar;
        private DataService _DetailArticleDownloader;
        private ArticleViewModel _DataArticle;
        private Android.Support.V7.Widget.ShareActionProvider _ShareAction;
        private WebView _WebView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.DetailArticleLayout);
            _ImageTitleHolder = FindViewById<ImageView>(Resource.Id.ImageTitleHolder);
            _ContentTextView = FindViewById<TextView>(Resource.Id.ContentTextView);
            _Progressbar = FindViewById<ProgressBar>(Resource.Id.ProgressBarOnDetail);
            //_WebView = FindViewById<WebView>(Resource.Id.WebView);
            //_WebView.Visibility = ViewStates.Invisible;
            //_WebView.Settings.JavaScriptEnabled = false;
            //_WebView.Settings.LoadWithOverviewMode = true;
            //_WebView.Settings.UseWideViewPort = true;
            
            string ids = Intent.GetStringExtra("IdForDetail");
            int id = int.Parse(ids);

            _DetailArticleDownloader = new DataService();
            _DetailArticleDownloader.GetDetailArticle(id);
            _DetailArticleDownloader.DownloadCompleted += OnDownloadCompleted;

            _DataArticle = new ArticleViewModel();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.menu, menu);

            if (_DataArticle.Post == null)
            {
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(false);
                menu.FindItem(Resource.Id.Bookmarks).SetVisible(false);
            }
            else
            {
                menu.FindItem(Resource.Id.ActionShare).SetEnabled(true);
                menu.FindItem(Resource.Id.Bookmarks).SetVisible(true);
                if (_DataArticle.Post.IsFavorite)
                {
                    menu.FindItem(Resource.Id.Bookmarks).SetIcon(Resources.GetDrawable(Resource.Drawable.ic_action_rating_important));
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
            switch(item.ItemId)
            {
                case Resource.Id.Bookmarks:
                    {
                        Toast.MakeText(this.BaseContext, "tes bookmark", ToastLength.Long);
                        _DataArticle.Post.IsFavorite = true;
                        BookmarksViewModel bookmarks = new BookmarksViewModel();
                        _DataArticle.Post.Attachments[0].Images.Full.Images.Recycle();
                        _DataArticle.Post.Attachments[0].Images.Full.Images.Dispose();
                        _DataArticle.Post.Attachments[0].Images.Full.Images = null;
                        ListUtils.SaveBookmarks(_DataArticle.Post);
                        break;
                    }
                default:
                    break;
            }

            base.OnOptionsItemSelected(item);
            return true;
        }

        public override void OnBackPressed()
        {
            if (_DataArticle.Post.Attachments[0].Images.Full.Images != null)
            {
                _DataArticle.Post.Attachments[0].Images.Full.Images.Recycle();
                _DataArticle.Post.Attachments[0].Images.Full.Images.Dispose();
            }
            _DataArticle = null;
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            this.Finish();
            base.OnBackPressed();
        }

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            _DetailArticleDownloader.DownloadCompleted -= OnDownloadCompleted;
            if (((DailySocial.Utils.DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((DailySocial.Utils.DownloadEventArgs)e).ResultDownload;
                _DataArticle = JsonConvert.DeserializeObject<ArticleViewModel>(raw);
                _DataArticle.Post.Title = HttpUtility.HtmlDecode(_DataArticle.Post.Title);
                _DataArticle.Post.Content = HttpUtility.HtmlDecode(_DataArticle.Post.Content);
                if (_DataArticle.Post.Attachments.Count > 0)
                {
                    _DataArticle.Post.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(_DataArticle.Post.Attachments[0].Images.Full.Url);
                }
                RunOnUiThread(() =>
                    {
                        InvalidateOptionsMenu();
                        _Progressbar.Activated = false;
                        _Progressbar.Visibility = ViewStates.Gone;
                        _ImageTitleHolder.SetImageBitmap(_DataArticle.Post.Attachments[0].Images.Full.Images);
                        _ContentTextView.Text = _DataArticle.Post.Content.Trim();
                        //_WebView.LoadDataWithBaseURL("", _DataArticle.Post.Content, "text/html", "UTF-8", "");
                        //_WebView.Activated = true;
                    });
            }
        }
    }
}