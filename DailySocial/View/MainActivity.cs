using DailySocial.Utils;
using DailySocial.View.Tabs;

using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using System;

namespace DailySocial
{
    [Activity(Label = "Daily Social", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : FragmentActivity, ViewPager.IOnPageChangeListener
    {
        private DataService _TopStoriesDownloader;
        private DataService _CategoriesDownloader;

        private TopStoriesFragment _TopStoriesFragment;
        private CategoriesFragment _CategoriesFragment;

        private ViewPager _ViewPager;

        protected override void OnCreate(Bundle bundle)
        {
            _TopStoriesDownloader = new DataService();
            _CategoriesDownloader = new DataService();

            _TopStoriesDownloader.GetTopStories();
            _TopStoriesDownloader.DownloadCompleted += _TopStoriesDownloader_DownloadCompleted;
            _CategoriesDownloader.GetCategories();
            _CategoriesDownloader.DownloadCompleted += _CategoriesDownloader_DownloadCompleted;

            RequestWindowFeature(WindowFeatures.ActionBar);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Get UI Control
            _ViewPager = FindViewById<ViewPager>(Resource.Id.FragmentContainer);

            //add fragment to view pager
            var adapter = new GenericFragmentPagerAdapter(SupportFragmentManager);
            _TopStoriesFragment = new TopStoriesFragment();
            _CategoriesFragment = new CategoriesFragment();
            adapter.AddFragment(_TopStoriesFragment);
            adapter.AddFragment(_CategoriesFragment);
            adapter.AddFragment(new BookmarksFragment());
            _ViewPager.Adapter = adapter;
            _ViewPager.SetOnPageChangeListener(this);

            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Top Stories"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Categories"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Bookmarks"));

            //show list to UI
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

        public override void OnLowMemory()
        {
            Log.Info("ds", "Low memory");
            _TopStoriesFragment.RecycleImage();
            base.OnLowMemory();
        }

        private void _CategoriesDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            _CategoriesDownloader.DownloadCompleted -= _CategoriesDownloader_DownloadCompleted;
            string raw;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw != null || raw.Length != 0)
                    _CategoriesFragment.UpdateListAdapter(raw);
            }
        }

        private void _TopStoriesDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            _TopStoriesDownloader.DownloadCompleted -= _TopStoriesDownloader_DownloadCompleted;
            string raw;
            if (((DownloadEventArgs)e).ResultDownload != null)
            {
                raw = ((DownloadEventArgs)e).ResultDownload;
                if (raw != null || raw.Length != 0)
                {
                    _TopStoriesFragment.UpdateListAdapter(raw);
                }
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
            if (ActionBar.SelectedNavigationIndex == 0)
            {
                _TopStoriesFragment.ShowList();
            }
            else if (ActionBar.SelectedNavigationIndex == 1)
            {
                _CategoriesFragment.ShowList();
            }
        }
    }
}