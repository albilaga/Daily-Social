using Android.App;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using DailySocial.Utils;
using DailySocial.View.Tabs;
using System;
using System.Threading.Tasks;

namespace DailySocial.View
{
    [Activity(Theme = "@style/Theme.ActionBarSize")]
    public class MainActivity : FragmentActivity, ViewPager.IOnPageChangeListener
    {
        private DataService _TopStoriesDownloader;
        private DataService _CategoriesDownloader;

        private TopStoriesFragment _TopStoriesFragment;
        private CategoriesFragment _CategoriesFragment;
        private BookmarksFragment _BookmarksFragment;

        private ViewPager _ViewPager;
        private bool _IsDataTopStoriesNull;
        private bool _IsDataCategoriesNull;

        protected override void OnCreate(Bundle bundle)
        {
            _TopStoriesDownloader = new DataService();
            _CategoriesDownloader = new DataService();

            _TopStoriesDownloader.GetTopStories();
            _TopStoriesDownloader.DownloadCompleted += OnTopStoriesDownloaderDownloadCompleted;
            _CategoriesDownloader.GetCategories();
            _CategoriesDownloader.DownloadCompleted += OnCategoriesDownloaderDownloadCompleted;

            RequestWindowFeature(WindowFeatures.ActionBar);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetDisplayShowCustomEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
            ActionBar.SetDisplayUseLogoEnabled(false);
            //ActionBar.SetCustomView(Resource.Layout.CustomActionBar);

            var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            var v = inflater.Inflate(Resource.Layout.CustomActionBar, null);
            ActionBar.SetCustomView(v, new ActionBar.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, GravityFlags.Center));

            var homeIcon = FindViewById(Android.Resource.Id.Home);
            ((Android.Views.View)homeIcon.Parent).Visibility = ViewStates.Gone;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Get UI Control
            _ViewPager = FindViewById<ViewPager>(Resource.Id.FragmentContainer);

            //add fragment to view pager
            var adapter = new GenericFragmentPagerAdapter(SupportFragmentManager);
            _TopStoriesFragment = new TopStoriesFragment();
            _CategoriesFragment = new CategoriesFragment();
            _BookmarksFragment = new BookmarksFragment();
            adapter.AddFragment(_TopStoriesFragment);
            adapter.AddFragment(_CategoriesFragment);
            adapter.AddFragment(_BookmarksFragment);
            _ViewPager.Adapter = adapter;
            _ViewPager.SetOnPageChangeListener(this);

            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Terbaru"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Kategori"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Favorit"));

            RunOnUiThread(ShowInternetAlertDialog);

            base.OnCreate(bundle);
        }

        protected override void OnResume()
        {
            if (_TopStoriesFragment.DataTopStories != null && _TopStoriesFragment.IsVisible)
            {
                _TopStoriesFragment.ShowList();
            }
            else
            {
                _TopStoriesDownloader.GetTopStories();
            }
            if (_CategoriesFragment.DataCategories != null && _CategoriesFragment.IsVisible)
            {
                _CategoriesFragment.ShowList();
            }
            else
            {
                _CategoriesDownloader.GetCategories();
            }
            base.OnResume();
        }

        public override void OnBackPressed()
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetIcon(Resource.Drawable.error);
            builder.SetTitle("Konfirmasi Keluar");
            builder.SetMessage("Apakah anda ingin keluar?");
            builder.SetCancelable(false);
            builder.SetPositiveButton("Ya", (sender, e) => Finish());
            builder.SetNegativeButton("Tidak", (sender, e) => { });
            var alertDialog = builder.Create();
            alertDialog.Show();
            //get title divider and styling it
            //got from here http://blog.supenta.com/2014/07/02/how-to-style-alertdialogs-like-a-pro/
            //int titleDividerId = Application.Resources.GetIdentifier("titleDivider", "id", "android");
            //Android.Views.View titleDivider = alertDialog.FindViewById(titleDividerId);
            //if (titleDivider != null)
            //{
            //    titleDivider.SetBackgroundColor(new Color(Color.ParseColor("#a8cf45")));
            //}
        }

        private void OnCategoriesDownloaderDownloadCompleted(object sender, EventArgs e)
        {
            _CategoriesDownloader.DownloadCompleted -= OnCategoriesDownloaderDownloadCompleted;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw.Length == 0) return;
                Task.Factory.StartNew(() => _CategoriesFragment.UpdateListAdapter(raw));
                ListUtils.SaveCategories(raw);
                _IsDataCategoriesNull = false;
            }
            else
            {
                _IsDataCategoriesNull = true;
              
            }
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
                _TopStoriesDownloader.DownloadCompleted += OnTopStoriesDownloaderDownloadCompleted;
                _TopStoriesDownloader.GetTopStories();
                _CategoriesDownloader.DownloadCompleted += OnCategoriesDownloaderDownloadCompleted;
                _CategoriesDownloader.GetCategories();
                _TopStoriesFragment.Reset();
                _CategoriesFragment.Reset();
            });
            builder.SetNegativeButton("Tidak", (send, eve) =>
            {
                _TopStoriesFragment.ShowList();
                _CategoriesFragment.ShowList();
            });
            var alertDialog = builder.Create();
            var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection == null) || !activeConnection.IsConnected)
            {
                alertDialog.Show();
            }
        }

        private void OnTopStoriesDownloaderDownloadCompleted(object sender, EventArgs e)
        {
            _TopStoriesDownloader.DownloadCompleted -= OnTopStoriesDownloaderDownloadCompleted;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                var raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw.Length == 0) return;
                Task.Factory.StartNew(() => _TopStoriesFragment.UpdateListAdapter(raw));
                ListUtils.SaveTopStories(raw);
                _IsDataTopStoriesNull = false;
            }
            else
            {
                _IsDataTopStoriesNull = true;
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            ActionBar.SetSelectedNavigationItem(position);
            switch (ActionBar.SelectedNavigationIndex)
            {
                case 0:
                    _TopStoriesFragment.ShowList();
                    break;

                case 1:
                    _CategoriesFragment.ShowList();
                    break;

                case 2:
                    _BookmarksFragment.ShowList();
                    break;
            }
        }
    }
}