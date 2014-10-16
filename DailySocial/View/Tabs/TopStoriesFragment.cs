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
        public ListView ListViewOnTopStories;
        public ProgressBar ProgressBarOnTopStories;
        public bool IsLoadedOnTopStories;
        public TopStoriesViewModel DataTopStories;

        public override void OnAttach(Activity activity)
        {
            Log.Info("ds", "on attach");
            DataTopStories = ListUtils.LoadTopStories();
            if (DataTopStories != null)
            {
                IsLoadedOnTopStories = true;
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
            if (!IsLoadedOnTopStories || raw != "")
            {
                Log.Info("ds", "Load top stories from web");
                DataTopStories = JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
            }
            DataTopStories.TempPosts = DataTopStories.Posts;
            DataTopStories.Posts = null;
            IsLoadedOnTopStories = true;
            ShowList();
            //if (DataTopStories.TempPosts[0].Attachments[0].Images.Full.Images == null)
            //{
            //    foreach (var x in DataTopStories.TempPosts)
            //    {
            //        if (x.Attachments.Count > 0)
            //        {
            //            Log.Info("ds", "decode image");
            //            x.Attachments[0].Images.Full.Images = ListUtils.GetImageBitmapFromUrl(x.Attachments[0].Images.Medium.Url);
            //        }
            //    }
            //    ShowList();
            //}
        }

        /// <summary>
        /// Bind data to UI
        /// </summary>
        public void ShowList()
        {
            if (Activity.ActionBar.SelectedNavigationIndex == 0)
            {
                if (IsLoadedOnTopStories)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        ListViewOnTopStories.Adapter = new TopStoriesAdapter(Activity, DataTopStories.TempPosts);
                        ProgressBarOnTopStories.Activated = false;
                    });
                }
            }
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            ListViewOnTopStories = view.FindViewById<ListView>(Resource.Id.ListView);
            ListViewOnTopStories.ItemClick += OnListItemClick;
            ProgressBarOnTopStories = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            view.FindViewById<TextView>(Resource.Id.TextView).Visibility = ViewStates.Gone;
            base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        public void RecycleImage()
        {
            if (DataTopStories.TempPosts != null)
            {
                foreach (var x in DataTopStories.TempPosts)
                {
                    if (x.Attachments[0].Images.Full.Images != null)
                    {
                        x.Attachments[0].Images.Full.Images.Recycle();
                        x.Attachments[0].Images.Full.Images.Dispose();
                    }
                }
            }
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(Activity.BaseContext, typeof(DetailArticleActivity));
            intent.PutExtra("IdForDetail", e.Id.ToString(CultureInfo.InvariantCulture));
            Activity.StartActivity(intent);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}