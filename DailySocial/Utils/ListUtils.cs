using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Widget;
using DailySocial.Models;
using DailySocial.ViewModel;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Threading;

namespace DailySocial.Utils
{
    public static class ListUtils
    {
        public static LruCache Cache { get; set; }

        public static int CalculateSimpleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            //raw height and with of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                //Calculate the largest inSampleSize value that is a power of 2 and keeps both height and width larger than the requested height and width
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }
            return inSampleSize;
        }

        /// <summary>
        /// Get Worker Bitmap which associated with particular Image View
        /// Function comes from <a href="https://developer.android.com/training/displaying-bitmaps/process-bitmap.html">Android Developer</a>
        /// </summary>
        /// <param name="imageView">image view which bitmapworkertask associated</param>
        /// <returns>Bitmap Worker Task Which work in particular ImageView</returns>
        public static BitmapWorkerTask GetBitmapWorkerTask(ImageView imageView)
        {
            if (imageView != null)
            {
                Drawable drawable = imageView.Drawable;
                if (drawable is AsyncDrawable)
                {
                    AsyncDrawable asyncDrawable = drawable as AsyncDrawable;
                    return asyncDrawable.BitmapWorkerTask;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if another running task is already associated with the Image View
        /// Function comes from <a href="https://developer.android.com/training/displaying-bitmaps/process-bitmap.html">Android Developer</a>
        /// </summary>
        /// <param name="url">url for image to download</param>
        /// <param name="imageView">image view to bind image</param>
        /// <returns></returns>
        public static bool CancelPotentialWork(string url, ImageView imageView)
        {
            BitmapWorkerTask bitmapWorkerTask = GetBitmapWorkerTask(imageView);
            if (bitmapWorkerTask != null)
            {
                string bitmapData = bitmapWorkerTask.Data;
                //if bitmapData is not yet set or it differs from the new data
                if (bitmapData == "" || bitmapData != url)
                {
                    //Cancel previous task
                    bitmapWorkerTask.Cancel(true);
                }
                else
                {
                    //the same work is already in progress
                    return false;
                }
            }
            //no task associated with the image view or an existing task was cancelled
            return true;
        }

        /// <summary>
        /// Load Bitmap In Background Thread
        /// Function comes from <a href="https://developer.android.com/training/displaying-bitmaps/process-bitmap.html">Android Developer</a>
        /// </summary>
        /// <param name="url">url where image hosted</param>
        /// <param name="imageView">image view where image binded</param>
        /// <param name="loadingImage">Image default when Image from url not yet downloaded</param>
        public static void LoadBitmap(string url, Bitmap loadingImage, ImageView imageView)
        {
            if (CancelPotentialWork(url, imageView))
            {
                BitmapWorkerTask task = new BitmapWorkerTask(imageView);
                AsyncDrawable asyncDrawable = new AsyncDrawable(Application.Context.Resources, loadingImage, task);
                Bitmap image = Cache.Get(url) as Bitmap;
                if (image != null)
                {
                    Log.Debug("ds", "image not null");
                    imageView.SetImageBitmap(image);
                }
                else
                {
                    imageView.SetImageDrawable(asyncDrawable);
                    task.Execute(url);
                }
            }
        }

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
                MethodBase currentMethod = MethodBase.GetCurrentMethod();
                if (currentMethod.DeclaringType != null)
                    Console.WriteLine("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}", currentMethod.DeclaringType.FullName, currentMethod.Name, ex.Message);
            }

            return imageBitmap;
        }

        public static string DateTimeToDateConverter(string value)
        {
            DateTime dateTime = DateTime.Parse(value);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("id-ID");
            var ci = Thread.CurrentThread.CurrentCulture;
            ci.DateTimeFormat.LongDatePattern = "dd MMMM yyyy";
            return dateTime.ToLongDateString();
        }

        #region Save Cache and Bookmarks

        private const string _FilenameBookmarksJson = "bookmarks.json";
        private const string _FilenameTopstoriesJson = "topstories.json";
        private const string _FilenameCategoriesJson = "categories.json";

        public static void SaveBookmarks(PostModel post)
        {
            BookmarksViewModel bookmarks = LoadBookmarks();
            if (bookmarks == null)
            {
                bookmarks = new BookmarksViewModel();
            }
            bookmarks.Bookmarks.Add(post);
            var json = JsonConvert.SerializeObject(bookmarks);
            IsoStorage.Save(_FilenameBookmarksJson, json);
        }

        public static BookmarksViewModel LoadBookmarks()
        {
            var raw = IsoStorage.Load<string>(_FilenameBookmarksJson);
            if (raw != null)
            {
                return JsonConvert.DeserializeObject<BookmarksViewModel>(raw);
            }
            return null;
        }

        public static void SaveTopStories(string raw)
        {
            IsoStorage.Save(_FilenameTopstoriesJson, raw);
        }

        public static TopStoriesViewModel LoadTopStories()
        {
            var raw = IsoStorage.Load<string>(_FilenameTopstoriesJson);
            if (raw != null)
            {
                return JsonConvert.DeserializeObject<TopStoriesViewModel>(raw);
            }
            return null;
        }

        public static void SaveCategories(string raw)
        {
            IsoStorage.Save(_FilenameCategoriesJson, raw);
        }

        public static CategoriesViewModel LoadCategories()
        {
            var raw = IsoStorage.Load<string>(_FilenameCategoriesJson);
            if (raw != null)
            {
                return JsonConvert.DeserializeObject<CategoriesViewModel>(raw);
            }
            return null;
        }

        #endregion Save Cache and Bookmarks
    }
}