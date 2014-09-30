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
using System.Net;
using Android.Util;

namespace DailySocial.Utils
{
    public class DataService
    {
        private const string _GetTopStoriesUrl = "http://api.dailysocial.net/api/?json=get_recent_posts";
        private const string _GetCategoriesUrl = "http://api.dailysocial.net/api/?json=get_category_index";
        private const string _GetListArticleByCategoriesUrl = "http://api.dailysocial.net/api/?json=get_category_posts&id=";
        private const string _GetDetailArticleUrl = "http://api.dailysocial.net/api/?json=get_post&id=";

        /// <summary>
        /// event when download json data completed
        /// </summary>
        public event EventHandler DownloadCompleted;

        public DataService()
        {
            WebRequest.DefaultWebProxy = null;
        }


        #region download data
        /// <summary>
        /// Get json data for top stories from web
        /// </summary>
        public void GetTopStories()
        {
            Log.Info("ds", "Get top stories");
            WebClient topStoriesClient = new WebClient();
            topStoriesClient.DownloadStringCompleted += data_DownloadStringCompleted;
            topStoriesClient.DownloadStringAsync(new Uri(_GetTopStoriesUrl));
        }

        /// <summary>
        /// Get json data for categories from web
        /// </summary>
        public void GetCategories()
        {
            Log.Info("ds", "Get categories");
            WebClient categoriesClient = new WebClient();
            categoriesClient.DownloadStringCompleted += data_DownloadStringCompleted;
            categoriesClient.DownloadStringAsync(new Uri(_GetCategoriesUrl));
        }

        /// <summary>
        /// Get json data for list of articles by category from web
        /// </summary>
        /// <param name="id">id from categories</param>
        public void GetArticlesByCategory(int id)
        {
            WebClient postByCategoryClient = new WebClient();
            postByCategoryClient.DownloadStringCompleted += data_DownloadStringCompleted;
            postByCategoryClient.DownloadStringAsync(new Uri(_GetListArticleByCategoriesUrl+id));
        }

        /// <summary>
        /// Get json data for detail of a article from web
        /// </summary>
        /// <param name="id">id from posts</param>
        public void GetDetailArticle(int id)
        {
            WebClient detailArticleClient = new WebClient();
            detailArticleClient.DownloadStringCompleted += data_DownloadStringCompleted;
            detailArticleClient.DownloadStringAsync(new Uri(_GetDetailArticleUrl + id));
        }

        /// <summary>
        /// activate event Download Completed when download data is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void data_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Log.Info("ds", "download completed 1");
            if(DownloadCompleted!=null)
            {
                var args = new DownloadEventArgs();
                //Log.Info("ds", e.Result);
                try
                {
                    args.ResultDownload = e.Result;
                }
                catch(Exception ex)
                {
                    args.ResultDownload = null;
                }
                DownloadCompleted(this, args);
            }
        }

        public string GetCacheBuster()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion
    }
}