using DailySocial.Models;

using Android.App;
using Android.Views;
using Android.Widget;

using System.Collections.Generic;

namespace DailySocial.View.Tabs.Adapter
{
    internal class CategoriesAdapter : BaseAdapter<CategoryModel>
    {
        private List<CategoryModel> _Category;
        private Activity _Context;

        public CategoriesAdapter(Activity context, List<CategoryModel> category)
            : base()
        {
            this._Context = context;
            this._Category = category;
        }

        /// <summary>
        /// Get Item ID
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override long GetItemId(int position)
        {
            return _Category[position].Id;
        }

        public override CategoryModel this[int position]
        {
            get
            {
                return _Category[position];
            }
        }

        public override int Count
        {
            get
            {
                return _Category.Count;
            }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var post = _Category[position];
            Android.Views.View view = convertView;
            if (view == null)
            {
                view = _Context.LayoutInflater.Inflate(Resource.Layout.SingleListCategoryLayout, parent, false);
            }
            view.FindViewById<TextView>(Resource.Id.NameCategory).Text = post.Title;
            return view;
        }
    }
}