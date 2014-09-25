using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DailySocial.Models;
using DailySocial.View.Tabs.Adapter;
using DailySocial.Utils;
using DailySocial.ViewModel;
using Android.Util;

namespace DailySocial.View.Tabs
{
    [Activity(Label = "TopStoriesActivity")]
    public class TopStoriesActivity : ListActivity
    {
        private ListView _ListView;
        private TopStoriesViewModel _DataTopStories; 
        private DataService _DataService;
        //private 
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _DataTopStories = new TopStoriesViewModel();
            _DataService = new DataService();

            SetContentView(Resource.Layout.ListTopStoriesLayout);
            //_ListView = FindViewById<ListView>(Resource.Id.List);

            _DataService.GetTopStories();
            _DataService.DownloadCompleted += _DataService_DownloadCompleted;

           
            
        }

        private void CreateList()
        {
            Log.Info("ds", "list");
            //list of top stories news go here
            //_ListView.Adapter = new TopStoriesAdapter(this, _DataTopStories.Posts);
            //_ListView.ItemClick += _ListView_ItemClick;
            //ListAdapter = new SimpleAdapter(this, _DataTopStories.Posts, Resource.Layout.SingleListTopStoriesLayout, new string[] { "Title", "Excerpt", "Attachments.Images.Full" }, new int[] { Resource.Id.Title, Resource.Id.News, Resource.Id.ImagePost });
            ListAdapter = new TopStoriesAdapter(this, _DataTopStories.Posts);
            
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
            if(raw!=null)
            {
                _DataTopStories = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
                CreateList();
                Log.Info("ds", "download completed");
            }
        }

        //protected override void OnListItemClick(ListView l, Android.Views.View v, int position, long id)
        //{
        //    var item = _DataTopStories.Posts[position];
        //    Toast.MakeText(this, item.Id, ToastLength.Short).Show();
        //    //base.OnListItemClick(l, v, position, id);
        //}
    }
}