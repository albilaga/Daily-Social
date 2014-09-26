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

namespace DailySocial.View.Tabs
{
    [Activity(Label = "CategoriesActivity")]
    public class CategoriesActivity : ListActivity
    {
        public CategoriesViewModel DataCategories { get; private set; }
        
        private DataService _DataService;
        private ProgressBar _ProgressBar;

        protected override void OnCreate(Bundle bundle)
        {
            DataCategories = new CategoriesViewModel();
            _DataService = new DataService();
            _DataService.GetCategories();
            _DataService.DownloadCompleted+=_DataService_DownloadCompleted;

            SetContentView(Resource.Layout.ListLayout);

            _ProgressBar=FindViewById<ProgressBar>(Resource.Id.progressBar);

            //When data still null enable loading 
            if(DataCategories==null)
            {
                _ProgressBar.Activated = true;
            }

            base.OnCreate(bundle);
        }

        void _DataService_DownloadCompleted(object sender, EventArgs e)
        {
            var raw = ((DownloadEventArgs)e).ResultDownload;
            if (raw != null)
            {
                DataCategories = JsonConvert.DeserializeObject<CategoriesViewModel>(raw);
                Log.Info("ds", "categories downloaded");
                //Update List View on UI Thread
                RunOnUiThread(() =>
                    {
                        ListAdapter = new CategoriesAdapter(this, DataCategories.Categories);
                        _ProgressBar.Activated = false;
                    });
            }
        }
    }
}