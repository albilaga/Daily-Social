using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using Object = Java.Lang.Object;

namespace DailySocial.Utils
{
    /// <summary>
    /// Bitmap Worker Task to download Images in background thread
    /// Class comes from <a href="https://developer.android.com/training/displaying-bitmaps/process-bitmap.html">Android Developer</a>
    /// </summary>
    public class BitmapWorkerTask : AsyncTask
    {
        private readonly WeakReference<ImageView> _ImageViewReference;

        public string Data
        {
            get;
            private set;
        }

        public BitmapWorkerTask(ImageView imageView)
        {
            if (ListUtils.Cache == null)
            {
                Log.Debug("ds", "cache null");
                var am = Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;
                if (am != null)
                {
                    Log.Debug("ds", "am not null");
                    int memoryClass = am.MemoryClass * 1024 * 1024;
                    ListUtils.Cache = new LruCache(memoryClass);
                }
            }
            _ImageViewReference = new WeakReference<ImageView>(imageView);
        }

        protected override Object DoInBackground(params Object[] @params)
        {
            Data = @params[0].ToString();
            Bitmap bmp;
            try
            {
                bmp = ListUtils.GetImageBitmapFromUrl(Data);
                if (bmp != null)
                {
                    ListUtils.Cache.Put(Data, bmp);
                }
            }
            catch (Exception e)
            {
                Log.Debug("ds", e.StackTrace);
                bmp = null;
            }
            return bmp;
        }

        protected override void OnPostExecute(Object result)
        {
            if (IsCancelled)
            {
                result = null;
            }
            if (_ImageViewReference != null && result != null)
            {
                ImageView imageView;
                if (_ImageViewReference.TryGetTarget(out imageView))
                {
                    var bitmapWorkerTask = ListUtils.GetBitmapWorkerTask(imageView);
                    if (this == bitmapWorkerTask)
                    {
                        imageView.SetImageBitmap(result as Bitmap);
                    }
                }
            }
            base.OnPostExecute(result);
        }
    }
}