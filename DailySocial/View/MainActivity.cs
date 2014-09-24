using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DailySocial.View.Tabs;

namespace DailySocial
{
    [Activity(Label = "DailySocial", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TabActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Add Tabs
            CreateTab(typeof(TopStoriesActivity), "topStories", "Top Stories");
            CreateTab(typeof(CategoriesActivity), "categories", "Categories");
            CreateTab(typeof(TopStoriesActivity), "bookmarks", "Bookmarks");
        }

        /// <summary>
        /// Add Tabs for Main Layout
        /// </summary>
        private void CreateTab(Type activityType, string tag, string label)
        {
            var intent = new Intent(this, activityType);
            intent.AddFlags(ActivityFlags.NewTask);

            var spec = TabHost.NewTabSpec(tag);
            spec.SetIndicator(label);
            spec.SetContent(intent);

            TabHost.AddTab(spec);
        }
    }
}

