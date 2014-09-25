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
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization;

namespace DailySocial.Utils
{
    public class IsoStorage
    {
        public static void Save<T>(string fileName, T item)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, storage))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(fileStream, item);
                }
            }
        }


        public static void Save(Stream imageStream, string fileName)
        {
            using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorage.FileExists(fileName))
                    isolatedStorage.DeleteFile(fileName);

                using (IsolatedStorageFileStream fileStream = isolatedStorage.CreateFile(fileName))
                {
                    imageStream.CopyTo(fileStream);
                    fileStream.Close();
                }
            }
        }

        public static T Load<T>(string fileName)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        return (T)serializer.ReadObject(fileStream);
                    }
                }
                else
                {
                    return default(T);
                }
            }
        }

        public static Stream LoadStream(string fileName)
        {
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (iso.FileExists(fileName))
                {
                    var stream = iso.OpenFile(fileName, FileMode.Open, FileAccess.Read);
                    return stream;
                }
                else
                {
                    return null;
                }
            }

        }

        public static bool Delete(string fileName)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
