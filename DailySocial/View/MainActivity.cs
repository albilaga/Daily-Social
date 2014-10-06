using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DailySocial.View.Tabs;
using Android.Util;
using DailySocial.ViewModel;
using DailySocial.Utils;
using Newtonsoft.Json;
using DailySocial.View.Tabs.Adapter;
using DailySocial.Models;
using System.Collections.Generic;
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace DailySocial
{
    [Activity(Label = "DailySocial", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity :FragmentActivity,ViewPager.IOnPageChangeListener
    {
        private DataService _TopStoriesDownloader;
        private DataService _CategoriesDownloader;

        private TopStoriesFragment _TopStoriesFragment;
        private CategoriesFragment _CategoriesFragment;

        public event EventHandler DownloadCancelled;


        private ViewPager _ViewPager;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Info("ds", "on create main activity");
        
            _TopStoriesDownloader = new DataService();
            _CategoriesDownloader = new DataService();

            //TopStoriesFragment fragmentTopStories=getloca

            _TopStoriesDownloader.GetTopStories();
            _TopStoriesDownloader.DownloadCompleted += _TopStoriesDownloader_DownloadCompleted;
            _CategoriesDownloader.GetCategories();
            _CategoriesDownloader.DownloadCompleted += _CategoriesDownloader_DownloadCompleted;

            RequestWindowFeature(WindowFeatures.ActionBar);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            _ViewPager = FindViewById<ViewPager>(Resource.Id.FragmentContainer);
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

            if (_TopStoriesFragment._DataTopStories != null && _TopStoriesFragment.IsVisible && _TopStoriesFragment._DataTopStories.TempPosts != null)
            {
                RunOnUiThread(() =>
                    {
                        _TopStoriesFragment._ProgressBar.Activated = false;
                        _TopStoriesFragment._ListView.Adapter = new TopStoriesAdapter(this, _TopStoriesFragment._DataTopStories.TempPosts);
                    });
            }
            if (_CategoriesFragment._DataCategories!=null && _CategoriesFragment._DataCategories.TempCategories != null && _CategoriesFragment.IsVisible)
            {
                RunOnUiThread(() =>
                {
                    _CategoriesFragment._ProgressBar.Activated = false;
                    _CategoriesFragment._ListView.Adapter = new CategoriesAdapter(this, _CategoriesFragment._DataCategories.TempCategories);
                });
            }
            base.OnCreate(bundle);
        }

        void _CategoriesDownloader_DownloadCompleted(object sender, EventArgs e)
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

        void _TopStoriesDownloader_DownloadCompleted(object sender, EventArgs e)
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

        protected override void OnDestroy()
        {
            
            base.OnDestroy();
        }


        public void OnPageScrollStateChanged(int state)
        {
            //throw new NotImplementedException();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            ActionBar.SetSelectedNavigationItem(position);
            Log.Info("ds", "tab index On Page Selected = " + ActionBar.SelectedNavigationIndex);
            if(ActionBar.SelectedNavigationIndex==0)
            {
                _TopStoriesFragment.ShowList();
            }
            else if(ActionBar.SelectedNavigationIndex==1)
            {
                _CategoriesFragment.ShowList();
            }
        }
    }
}

