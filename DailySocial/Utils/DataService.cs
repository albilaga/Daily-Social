using Android.Util;

using System;
using System.Net;
using System.Text;

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
            var topStoriesClient = new CustomWebClient(10000) { Encoding = Encoding.UTF8 };
            topStoriesClient.DownloadStringCompleted += OnDownloadStringCompleted;
            topStoriesClient.DownloadStringAsync(new Uri(_GetTopStoriesUrl+GetCacheBuster()));
        }

        /// <summary>
        /// Get json data for categories from web
        /// </summary>
        public void GetCategories()
        {
            Log.Info("ds", "Get categories");
            var categoriesClient = new CustomWebClient(10000) { Encoding = Encoding.UTF8 };
            categoriesClient.DownloadStringCompleted += OnDownloadStringCompleted;
            categoriesClient.DownloadStringAsync(new Uri(_GetCategoriesUrl+GetCacheBuster()));
        }

        /// <summary>
        /// Get json data for list of articles by category from web
        /// </summary>
        /// <param name="id">id from categories</param>
        public void GetArticlesByCategory(int id)
        {
            Log.Debug("ds", "Get Articles from category = " + id);
            var postByCategoryClient = new CustomWebClient(10000) { Encoding = Encoding.UTF8 };
            postByCategoryClient.DownloadStringCompleted += OnDownloadStringCompleted;
            postByCategoryClient.DownloadStringAsync(new Uri(_GetListArticleByCategoriesUrl + id+GetCacheBuster()));
        }

        /// <summary>
        /// Get json data for detail of a article from web
        /// </summary>
        /// <param name="id">id from posts</param>
        public void GetDetailArticle(int id)
        {
            var detailArticleClient = new CustomWebClient(10000) { Encoding = Encoding.UTF8 };
            detailArticleClient.DownloadStringCompleted += OnDownloadStringCompleted;
            detailArticleClient.DownloadStringAsync(new Uri(_GetDetailArticleUrl + id+GetCacheBuster()));
        }

        /// <summary>
        /// activate event Download Completed when download data is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.Debug("ds", e.Error.Message);
            }
            var wc = sender as WebClient;
            Log.Info("ds", "download completed 1");
            if (wc != null)
            {
                wc.DownloadStringCompleted -= OnDownloadStringCompleted;
                wc.Dispose();
            }
            if (DownloadCompleted == null) return;
            var args = new DownloadEventArgs();
            try
            {
                args.ResultDownload = e.Result;
                Log.Info("ds", e.Result);
            }
            catch (Exception)
            {
                args.ResultDownload = null;
            }
            DownloadCompleted.Invoke(this, args);
        }

        private static string GetCacheBuster()
        {
            var sb = new StringBuilder("&cache=");
            sb.Append(Guid.NewGuid().ToString());
            return sb.ToString();
        }

        #endregion download data
    }
}