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
using Android.Graphics;
using System.Net;
using DailySocial.Utils;

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
        }

        /// <summary>
        /// Get Item Id
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override long GetItemId(int position)
        {
            return _Posts[position].Id;
        }

        public override PostModel this[int position]
        {
            get 
            {
                return _Posts[position]; 
            }
        }

        public override int Count
        {
            get 
            {
                return _Posts.Count; 
            }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var post=_Posts[position];
            Android.Views.View view = convertView;
            if (view == null)
            {
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SingleListTopStoriesLayout, parent,false);
            }
            view.FindViewById<TextView>(Resource.Id.Title).Text = post.Title;
            view.FindViewById<TextView>(Resource.Id.News).Text = post.Excerpt;
            view.FindViewById<ImageView>(Resource.Id.ImagePost).SetImageBitmap(post.Attachments[0].Images.Full.Images);
            return view;
        }
    }
}