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

namespace DailySocial
{
    [Activity(Label = "DailySocial", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : TabActivity
    {
        private ListView _ListView;
        private TopStoriesViewModel _DataTopStories;
        private DataService _DataService;
        private Android.Views.View _MainView;
        //private Intent _Context;
        private Context _Context;

        public static ListView ListView { get; private set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Add Tabs
            CreateTab(typeof(TopStoriesActivity), "topStories", "Top Stories");
            CreateTab(typeof(CategoriesActivity), "categories", "Categories");
            CreateTab(typeof(TopStoriesActivity), "bookmarks", "Bookmarks");
            
            ListView = FindViewById<ListView>(Resource.Id.ListView);
            Log.Info("ds", "List View");
            TabHost.TabChanged += TabHost_TabChanged;
            //Intent.PutExtra("myClass", this.Intent);
            //var activity = new Intent(this, typeof(TopStoriesActivity));
            //activity.PutExtra("myData", ListView);

            _DataTopStories = new TopStoriesViewModel();
            _DataService = new DataService();
            //_DataService.GetTopStories();
            //_DataService.DownloadCompleted += _DataService_DownloadCompleted;
        }

        private void CreateList()
        {
            Log.Info("ds", "list");
            //list of top stories news go here
            //_ListView.Adapter = new TopStoriesAdapter(this, _DataTopStories.Posts);
            //_ListView.ItemClick += _ListView_ItemClick;
            //ListAdapter = new SimpleAdapter(this, _DataTopStories.Posts, Resource.Layout.SingleListTopStoriesLayout, new string[] { "Title", "Excerpt", "Attachments.Images.Full" }, new int[] { Resource.Id.Title, Resource.Id.News, Resource.Id.ImagePost });
            //ListAdapter = new TopStoriesAdapter(this, _DataTopStories.Posts);
            //SetContentView(ListAdapter.GetView();
            //_ListView = FindViewById<ListView>(Resource.Id.ListView);
            //ListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, new string[] { "tes1", "tes2", "tes3" });
            ListView.Adapter = new TopStoriesAdapter(this, _DataTopStories.Posts);
            Log.Info("ds", "set adapter");
            ListView.ItemClick += _ListView_ItemClick;
            //set

        }

        void _ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var t = _DataTopStories.Posts[e.Position];
            Toast.MakeText(this, t.Title, Android.Widget.ToastLength.Short).Show();
        }

        void _DataService_DownloadCompleted(object sender, EventArgs e)
        {
            var raw = ((DownloadEventArgs)e).ResultDownload;
            if (raw != null)
            {
                _DataTopStories = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                CreateList();
                Log.Info("ds", "download completed");
            }
        }


        void TabHost_TabChanged(object sender, TabHost.TabChangeEventArgs e)
        {
            if (TabHost.CurrentTab == 0)
            {
                Log.Info("ds", "Top stories");
            }
            else if(TabHost.CurrentTab==1)
            {
                Log.Info("ds", "Categories");
            }
            else if(TabHost.CurrentTab==2)
            {
                Log.Info("ds", "Bookmark");
            }
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

