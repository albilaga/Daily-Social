using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs.Adapter;
using DailySocial.ViewModel;

namespace DailySocial.View.Tabs
{
    public class BookmarksFragment : Android.Support.V4.App.Fragment
    {
        public BookmarksViewModel Bookmarks;
        public ListView ListViewOnBookmarks;
        public ProgressBar ProgressBarOnBookmarks;
        public TextView TextViewOnBookmarks;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Bookmarks = ListUtils.LoadBookmarks();
            base.OnActivityCreated(savedInstanceState);
        }

        public void ShowList()
        {
            Activity.RunOnUiThread(() =>
            {
                if (Bookmarks == null || Bookmarks.Bookmarks.Count == 0)
                {
                    TextViewOnBookmarks.Text = "No Data To Display";
                    ListViewOnBookmarks.Visibility = ViewStates.Gone;
                    ProgressBarOnBookmarks.Visibility = ViewStates.Gone;
                }
                else
                {
                    ListViewOnBookmarks.Adapter = new TopStoriesAdapter(Activity, Bookmarks.Bookmarks);
                    ProgressBarOnBookmarks.Activated = false;
                    TextViewOnBookmarks.Visibility = ViewStates.Gone;
                }
            });
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            ListViewOnBookmarks = view.FindViewById<ListView>(Resource.Id.ListView);
            ListViewOnBookmarks.ItemClick += OnListItemClick;
            ProgressBarOnBookmarks = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            TextViewOnBookmarks = view.FindViewById<TextView>(Resource.Id.TextView);
            base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(Activity.BaseContext, typeof(DetailArticleActivity));
            intent.PutExtra("IdForDetail", e.Id.ToString());
            Activity.StartActivity(intent);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}