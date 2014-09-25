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
using Android.Util;

namespace DailySocial.View.Tabs.Adapter
{
    class TopStoriesAdapter : BaseAdapter<PostModel>
    {
        private List<PostModel> _Posts;
        private Activity _Context;
        public TopStoriesAdapter(Activity context,List<PostModel> posts) : base()
        {
            this._Context = context;
            this._Posts = posts;
            //Log.Info("ds",_Context.)
        }

        public override long GetItemId(int position)
        {
            Log.Info("ds", "pos:" +position.ToString());
            return 0;
        }

        public override PostModel this[int position]
        {
            get 
            {
                Log.Info("ds","id "+ _Posts[position].Id.ToString());
                return _Posts[position]; 
            }
        }

        public override int Count
        {
            get 
            {
                Log.Info("ds","count"+_Posts.Count.ToString());
                return _Posts.Count; 
            }
        } 

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            Log.Info("ds", "get view");
            var post=_Posts[position];
            //Android.Views.View view;
            Android.Views.View view = convertView;
            if(view==null)
            {
                Log.Info("ds", "view null");
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SingleListTopStoriesLayout, null);
                //view = _Context.LayoutInflater.Inflate(Resource.Layout.SingleListTopStoriesLayout,null);
            }
            view.FindViewById<TextView>(Resource.Id.Title).Text = post.Title;
            view.FindViewById<TextView>(Resource.Id.News).Text = post.Excerpt;
            view.FindViewById<ImageView>(Resource.Id.ImagePost).SetImageURI( Android.Net.Uri.Parse(post.Attachments[0].Images.Full.Url));
            return view;
        }


    }
}