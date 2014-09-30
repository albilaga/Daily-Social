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
    public class MainActivity :FragmentActivity
    {
        private DataService _TopStoriesDownloader;
        private DataService _CategoriesDownloader;

        private TopStoriesFragment _TopStoriesFragment;
        private CategoriesFragment _CategoriesFragment;

        private CategoriesViewModel _DataCategories;// { get; private set; }
        private TopStoriesViewModel _DataTopStories;// { get; private set; }

        public ViewPager _ViewPager;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Info("ds", "on create main activity");
            _DataCategories = new CategoriesViewModel();
            _DataTopStories = new TopStoriesViewModel();

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
            _ViewPager.SetOnPageChangeListener(new ViewPageListenerForActionBar(ActionBar));

           

            //Add Tabs
            //CreateTab("Top Stories",_TopStoriesFragment);
            //CreateTab("Categories",_CategoriesFragment);
            //CreateTab("Bookmarks",new BookmarksFragment());

            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Top Stories"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Categories"));
            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar, "Bookmarks"));

            base.OnCreate(bundle);
        }

        void _CategoriesDownloader_DownloadCompleted(object sender, EventArgs e)
        {
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

        /// <summary>
        /// Add Tabs for Main Layout
        /// </summary>
        private void CreateTab(string label, Android.Support.V4.App.Fragment fragment)
        {
            var tab = ActionBar.NewTab();
            tab.SetText(label);
            tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e)
            {
                Log.Info("ds", "tab = " + ActionBar.SelectedNavigationIndex.ToString());
                _ViewPager.SetCurrentItem(ActionBar.SelectedNavigationIndex, false);
                //e.FragmentTransaction.Rem
                //e.FragmentTransaction.Replace(Resource.Id.FragmentContainer, (Android.App.Fragment)fragment);
            };

            ActionBar.AddTab(_ViewPager.GetViewPageTab(ActionBar,label));
        }

    }
}

