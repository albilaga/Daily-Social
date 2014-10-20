using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using DailySocial.Utils;
using DailySocial.View.Tabs.Adapter;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using System.Globalization;
using System.Threading.Tasks;

namespace DailySocial.View.Tabs
{
    public class TopStoriesFragment : Android.Support.V4.App.Fragment
    {
        private ListView _ListViewOnTopStories;
        private ProgressBar _ProgressBarOnTopStories;
        private bool _IsLoadedOnTopStories;
        public TopStoriesViewModel DataTopStories;

        public override void OnAttach(Activity activity)
        {
            Log.Info("ds", "on attach");
            DataTopStories = ListUtils.LoadTopStories();
            if (DataTopStories != null)
            {
                _IsLoadedOnTopStories = true;
                Task.Factory.StartNew(() => UpdateListAdapter(""));
                //UpdateListAdapter("");
            }
            base.OnAttach(activity);
        }

        /// <summary>
        /// Processing data from web to view model and update it to view
        /// </summary>
        /// <param name="raw">data raw from web</param>
        public void UpdateListAdapter(string raw)
        {
            if (!_IsLoadedOnTopStories || raw != "")
            {
                Log.Info("ds", "Load top stories from web");
                DataTopStories = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
            }
            DataTopStories.TempPosts = DataTopStories.Posts;
            DataTopStories.Posts = null;
            _IsLoadedOnTopStories = true;
            ShowList();
        }

        /// <summary>
        /// Bind data to UI
        /// </summary>
        public void ShowList()
        {
            if (Activity.ActionBar.SelectedNavigationIndex != 0) return;
            if (_IsLoadedOnTopStories && DataTopStories.TempPosts != null)
            {
                Activity.RunOnUiThread(() =>
                {
                    _ListViewOnTopStories.Adapter = new TopStoriesAdapter(Activity, DataTopStories.TempPosts);
                });
            }
            Activity.RunOnUiThread(() =>
            {
                _ProgressBarOnTopStories.Visibility = ViewStates.Gone;
            });
        }

        public void Reset()
        {
            if (Activity.ActionBar.SelectedNavigationIndex != 0) return;
            Activity.RunOnUiThread(() =>
            {
                _ProgressBarOnTopStories.Visibility = ViewStates.Visible;
                _ProgressBarOnTopStories.Activated = true;
            });
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            _ListViewOnTopStories = view.FindViewById<ListView>(Resource.Id.ListView);
            _ListViewOnTopStories.ItemClick += OnListItemClick;
            _ProgressBarOnTopStories = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            view.FindViewById<TextView>(Resource.Id.TextView).Visibility = ViewStates.Gone;
            Activity.RunOnUiThread(() =>
            {
                _ProgressBarOnTopStories.Visibility = ViewStates.Visible;
                _ProgressBarOnTopStories.Activated = true;
            })
            ;
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