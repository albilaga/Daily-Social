using DailySocial.ViewModel;
using DailySocial.Utils;
using DailySocial.View.Tabs.Adapter;
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
using Android.Util;
using Android.Support.V4.App;

namespace DailySocial.View.Tabs
{
    public class CategoriesFragment : Android.Support.V4.App.Fragment
    {
        public ListView _ListView;
        public ProgressBar _ProgressBar;
        public bool _IsLoaded = false;
        public CategoriesViewModel _DataCategories;

        public override void OnAttach(Activity activity)
        {
            _DataCategories = new CategoriesViewModel();
            base.OnAttach(activity);
        }
        public void UpdateListAdapter(string raw)
        {
            if (raw.Length != 0)
            {
                _DataCategories = JsonConvert.DeserializeObject<CategoriesViewModel>(raw);
                _DataCategories.TempCategories = _DataCategories.Categories;
                Log.Info("ds", "categories downloaded");
                _IsLoaded = true;
                if (this.IsVisible)
                {
                    Log.Info("ds", "Categories fragment visible");
                    if (_IsLoaded)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            _ListView.Adapter = new CategoriesAdapter(Activity, _DataCategories.Categories);
                            _ProgressBar.Activated = false;
                        });
                    }
                }
            }
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Log.Info("ds", "on categories");
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            _ListView = view.FindViewById<ListView>(Resource.Id.ListView);
            _ListView.ItemClick += _ListView_ItemClick;
            _ProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            if (this.IsVisible)
            {
                Log.Info("ds", "Categories fragment visible");
                if (_IsLoaded)
                    Activity.RunOnUiThread(() =>
                        {
                            _ProgressBar.Activated = false;
                            _ListView.Adapter = new CategoriesAdapter(Activity, _DataCategories.Categories);
                        });
            }
            base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        void _ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Activity.StartActivity(typeof(ArticlesByCategoryActivity));
            Activity.Finish(); 
        }

    }
}