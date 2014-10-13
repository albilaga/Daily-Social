﻿using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using DailySocial.Utils;
using DailySocial.View.Tabs;
using System;
using System.Threading.Tasks;

namespace DailySocial
{
    [Activity(Label = "Daily Social", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
    public class MainActivity : FragmentActivity, ViewPager.IOnPageChangeListener
    {
        private DataService _TopStoriesDownloader;
        private DataService _CategoriesDownloader;

        private TopStoriesFragment _TopStoriesFragment;
        private CategoriesFragment _CategoriesFragment;
        private BookmarksFragment _BookmarksFragment;

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
            _BookmarksFragment = new BookmarksFragment();
            adapter.AddFragment(_TopStoriesFragment);
            adapter.AddFragment(_CategoriesFragment);
            adapter.AddFragment(_BookmarksFragment);
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
                {
                    Task.Factory.StartNew(() => _CategoriesFragment.UpdateListAdapter(raw));
                    ListUtils.SaveCategories(raw);
                }
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
                    Task.Factory.StartNew(() => _TopStoriesFragment.UpdateListAdapter(raw));
                    ListUtils.SaveTopStories(raw);
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
            else if (ActionBar.SelectedNavigationIndex == 2)
            {
                _BookmarksFragment.ShowList();
            }
        }
    }
}