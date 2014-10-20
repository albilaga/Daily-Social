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
    public class CategoriesFragment : Android.Support.V4.App.Fragment
    {
        private ListView _ListViewOnCategories;
        private ProgressBar _ProgressBarOnCategories;
        private bool _IsLoadedOnCategories;
        public CategoriesViewModel DataCategories;

        public override void OnAttach(Activity activity)
        {
            DataCategories = ListUtils.LoadCategories();
            if (DataCategories != null)
            {
                _IsLoadedOnCategories = true;
                Task.Factory.StartNew(() => UpdateListAdapter(""));
            }
            base.OnAttach(activity);
        }

        /// <summary>
        /// Processing data from web to view model and update it to view
        /// </summary>
        /// <param name="raw">data raw from web</param>
        public void UpdateListAdapter(string raw)
        {
            if (!_IsLoadedOnCategories || raw != "")
            {
                Log.Info("ds", "Load categories from web");
                DataCategories = JsonConvert.DeserializeObject<CategoriesViewModel>(raw);
                _IsLoadedOnCategories = true;
            }
            ShowList();
        }

        /// <summary>
        /// Bind data to UI
        /// </summary>
        public void ShowList()
        {
            if (Activity.ActionBar.SelectedNavigationIndex != 1) return;
            if (_IsLoadedOnCategories)
            {
                Activity.RunOnUiThread(() =>
                {
                    _ListViewOnCategories.Adapter = new CategoriesAdapter(Activity, DataCategories.Categories);
                });
            }
            Activity.RunOnUiThread(() =>
            {
                _ProgressBarOnCategories.Visibility = ViewStates.Gone;
            });
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListLayout, container, false);
            _ListViewOnCategories = view.FindViewById<ListView>(Resource.Id.ListView);
            _ListViewOnCategories.ItemClick += OnListItemClick;
            _ProgressBarOnCategories = view.FindViewById<ProgressBar>(Resource.Id.ProgressBar);
            view.FindViewById<TextView>(Resource.Id.TextView).Visibility = ViewStates.Gone;
            base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        /// <summary>
        /// event when list view clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(Activity.BaseContext, typeof(ArticlesByCategoryActivity));
            intent.PutExtra("IdFromCategories", e.Id.ToString(CultureInfo.InvariantCulture));
            intent.PutExtra("TitleFromCategories", DataCategories.Categories[e.Position].Title);
            Activity.StartActivity(intent);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public void Reset()
        {
            if (Activity.ActionBar.SelectedNavigationIndex != 1) return;
            Activity.RunOnUiThread(() =>
            {
                _ProgressBarOnCategories.Visibility = ViewStates.Visible;
                _ProgressBarOnCategories.Activated = true;
            });
        }
    }
}