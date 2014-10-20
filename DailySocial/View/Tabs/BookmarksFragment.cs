using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs.Adapter;
using DailySocial.ViewModel;
using System.Globalization;

namespace DailySocial.View.Tabs
{
    public class BookmarksFragment : Android.Support.V4.App.Fragment
    {
        private BookmarksViewModel _Bookmarks;
        private ListView _ListViewOnBookmarks;
        private ProgressBar _ProgressBarOnBookmarks;
        private TextView _TextViewOnBookmarks;

        public override void OnAttach(Activity activity)
        {
            _Bookmarks = ListUtils.LoadBookmarks();
            base.OnAttach(activity);
        }

        public void ShowList()
        {
            if (Activity.ActionBar.SelectedNavigationIndex == 2)
            {
                Activity.RunOnUiThread(() =>
                {
                    if (_Bookmarks == null || _Bookmarks.Bookmarks.Count == 0)
                    {
                        _TextViewOnBookmarks.Visibility = ViewStates.Visible;
                        _ListViewOnBookmarks.Visibility = ViewStates.Gone;
                        _ProgressBarOnBookmarks.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        _ListViewOnBookmarks.Adapter = new TopStoriesAdapter(Activity, _Bookmarks.Bookmarks);
                        _ProgressBarOnBookmarks.Visibility = ViewStates.Gone;
                        _TextViewOnBookmarks.Visibility = ViewStates.Gone;
                    }
                });
            }
        }

        public override void OnResume()
        {
            _Bookmarks = ListUtils.LoadBookmarks();
            ShowList();
            base.OnResume();
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            _ListViewOnBookmarks = view.FindViewById<ListView>(Resource.Id.ListView);
            _ListViewOnBookmarks.ItemClick += OnListItemClick;
            _ProgressBarOnBookmarks = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            _TextViewOnBookmarks = view.FindViewById<TextView>(Resource.Id.TextView);
            base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(Activity.BaseContext, typeof(DetailArticleActivity));
            intent.PutExtra("IdForDetail", e.Id.ToString(CultureInfo.InvariantCulture));
            Activity.StartActivity(intent);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}