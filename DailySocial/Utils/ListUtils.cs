using Android.Graphics;

using System;
using System.Net;
using System.Reflection;

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
    }
}