using DailySocial.Models;
using DailySocial.ViewModel;
using Newtonsoft.Json;

using Android.Graphics;

using System;
using System.Net;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace DailySocial.Utils
{
    public static class ListUtils
    {
        /// <summary>
        /// Convert image from URL to bitmap
        /// </summary>
        /// <param name="url">url where the image hosted</param>
        /// <returns></returns>
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase currentMethod = MethodInfo.GetCurrentMethod();
                Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                    , currentMethod.DeclaringType.FullName
                    , currentMethod.Name
                    , ex.Message));
            }

            return imageBitmap;
        }

        //public static string BitmapToBase64(Bitmap image)
        //{
        //    //Bitmap imagex = image;
        //    //Stream stream = new MemoryStream();
        //    //imagex.

        //}

        #region Save Cache and Bookmarks
        private const string FILENAME_BOOKMARKS_JSON = "bookmarks.json";
        private const string FILENAME_TOPSTORIES_JSON = "topstories.json";
        private const string FILENAME_CATEGORIES_JSON = "categories.json";
        
        public static void SaveBookmarks(PostModel post)
        {
            BookmarksViewModel bookmarks = ListUtils.LoadBookmarks();
            if(bookmarks==null)
            {
                bookmarks = new BookmarksViewModel();
            }
            bookmarks.Bookmarks.Add(post);
            var json = JsonConvert.SerializeObject(bookmarks);
            IsoStorage.Save<string>(FILENAME_BOOKMARKS_JSON, json);
        }

        public static BookmarksViewModel LoadBookmarks()
        {
            var raw = IsoStorage.Load<string>(FILENAME_BOOKMARKS_JSON);
            if (raw != null)
            {
                return JsonConvert.DeserializeObject<BookmarksViewModel>(raw);
            }
            return null;
        }

        public static void SaveTopStories(string raw)
        {
            IsoStorage.Save<string>(FILENAME_TOPSTORIES_JSON, raw);
        }

        public static TopStoriesViewModel LoadTopStories()
        {
            var raw = IsoStorage.Load<string>(FILENAME_TOPSTORIES_JSON);
            if(raw!=null)
            {
                return JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
            }
            return null;
        }

        public static void SaveCategories(string raw)
        {
            IsoStorage.Save<string>(FILENAME_CATEGORIES_JSON, raw);
        }

        public static CategoriesViewModel LoadCategories()
        {
            var raw = IsoStorage.Load<string>(FILENAME_CATEGORIES_JSON);
            if(raw!=null)
            {
                return JsonConvert.DeserializeObject<CategoriesViewModel>(raw);
            }
            return null;
        }

        #endregion
    }
}