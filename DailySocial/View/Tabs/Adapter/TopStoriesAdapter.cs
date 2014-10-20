using Android.Graphics;
using DailySocial.Models;

using Android.App;
using Android.Views;
using Android.Widget;

using System.Collections.Generic;
using DailySocial.Utils;

namespace DailySocial.View.Tabs.Adapter
{
    internal class TopStoriesAdapter : BaseAdapter<PostModel>
    {
        private readonly List<PostModel> _Posts;
        private readonly Activity _Context;

        public TopStoriesAdapter(Activity context, List<PostModel> posts)
        {
            _Context = context;
            _Posts = posts;
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
            var post = _Posts[position];
            Android.Views.View view = convertView ??
                                      _Context.LayoutInflater.Inflate(Resource.Layout.SingleListTopStoriesLayout, parent, false);
            view.FindViewById<TextView>(Resource.Id.AuthorAndDate).Text = string.Format("{0} | {1}", post.Author.Name, post.LongDateTime);
            view.FindViewById<TextView>(Resource.Id.Title).Text = post.TitleDecode;
            if (post.Attachments.Count != 0)
            {
                ListUtils.LoadBitmap(post.Attachments[0].Images.Medium.Url,BitmapFactory.DecodeResource(Application.Context.Resources,Resource.Drawable.defaultimage), view.FindViewById<ImageView>(Resource.Id.ImagePost));
            }
            return view;
        }
    }
}